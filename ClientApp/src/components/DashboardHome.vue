<template>
  <div class="dashboard-home">
    <!-- 頁面標題 -->
    <div class="page-header">
      <h2 class="page-title">儀表板</h2>
      <p class="page-description">歡迎使用後台管理系統</p>
    </div>

    <!-- 統計卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.totalForms }}</div>
          <div class="stat-label">總表單數</div>
        </div>
        <div class="stat-icon">📝</div>
      </div>

      <div class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.todayForms }}</div>
          <div class="stat-label">今日新增</div>
        </div>
        <div class="stat-icon">📈</div>
      </div>

      <div class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.weekForms }}</div>
          <div class="stat-label">本週新增</div>
        </div>
        <div class="stat-icon">📊</div>
      </div>

      <div class="stat-card">
        <div class="stat-content">
          <div class="stat-number">{{ stats.monthForms }}</div>
          <div class="stat-label">本月新增</div>
        </div>
        <div class="stat-icon">📅</div>
      </div>
    </div>

    <div class="charts-grid">
      <div class="chart-card">
        <div class="chart-header">
          <h4 class="chart-title">表單填寫趨勢</h4>

          <div class="chart-tabs">
            <button
                :class="{ active: chartMode === '7d' }"
                @click="switchChart('7d')"
            >
              近 7 天
            </button>
            <button
                :class="{ active: chartMode === '30d' }"
                @click="switchChart('30d')"
            >
              一個月內
            </button>
            <button
                :class="{ active: chartMode === 'year' }"
                @click="switchChart('year')"
            >
              年度
            </button>
          </div>
        </div>

        <canvas ref="dailyChart"></canvas>
      </div>
    </div>
    <!-- 表單列表 -->
    <div class="recent-section">
      <div class="section-header">
        <h3 class="section-title">提交的表單</h3>
      </div>

      <div class="recent-forms">
        <div v-if="loading" class="loading">
          載入中...
        </div>

        <div v-else-if="pagedForms.length === 0" class="empty-state">
          <div class="empty-icon">📄</div>
          <p>暫無表單資料</p>
        </div>

        <div v-else class="forms-table">
          <div class="table-header">
            <div class="header-cell">姓名</div>
            <div class="header-cell">Email</div>
            <div class="header-cell">提交時間</div>
            <div class="header-cell">操作</div>
          </div>

          <div
              v-for="form in pagedForms"
              :key="form.id"
              class="table-row"
          >
            <div class="table-cell">{{ form.fullName }}</div>
            <div class="table-cell">{{ form.email }}</div>
            <div class="table-cell">{{ formatDate(form.createdAt) }}</div>
            <div class="table-cell">
              <router-link
                  :to="`/dashboard/forms/${form.id}`"
                  class="view-btn"
              >
                查看
              </router-link>
            </div>
          </div>

          <!-- 分頁 -->
          <div class="pagination">
            <button
                :disabled="currentPage === 1"
                @click="currentPage--"
            >
              上一頁
            </button>

            <button
                v-for="page in totalPages"
                :key="page"
                :class="{ active: page === currentPage }"
                @click="currentPage = page"
            >
              {{ page }}
            </button>

            <button
                :disabled="currentPage === totalPages"
                @click="currentPage++"
            >
              下一頁
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted,nextTick } from 'vue'
import { apiGet } from '../utils/api.js'
import { Chart } from 'chart.js/auto'

// ===== 狀態 =====
const loading = ref(true)

const stats = ref({
  totalForms: 0,
  todayForms: 0,
  weekForms: 0,
  monthForms: 0
})

const allForms = ref([])

// 分頁設定
const pageSize = 10
const currentPage = ref(1)

const dailyChart = ref(null)
const chartMode = ref('7d') // '7d' | '30d' | 'year'
let dailyChartInstance = null

// ===== 生命週期 =====
onMounted(() => {
  loadDashboardData()
})

// ===== API =====
const loadDashboardData = async () => {
  try {
    loading.value = true

    const forms = await apiGet('/api/Form')
    allForms.value = forms

    // === 統計 ===
    const now = new Date()
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
    const weekAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000)
    const monthAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000)

    stats.value = {
      totalForms: forms.length,
      todayForms: forms.filter(f => new Date(f.createdAt) >= today).length,
      weekForms: forms.filter(f => new Date(f.createdAt) >= weekAgo).length,
      monthForms: forms.filter(f => new Date(f.createdAt) >= monthAgo).length
    }
    await nextTick()
    renderCharts()
  } catch (err) {
    console.error('載入 Dashboard 失敗', err)
  } finally {
    loading.value = false
  }
}

// ===== computed =====
const totalPages = computed(() =>
    Math.ceil(allForms.value.length / pageSize)
)

const switchChart = async (mode) => {
  chartMode.value = mode
  await nextTick()
  renderCharts()
}



const renderCharts = () => {
  if (chartMode.value === '7d') {
    renderLastDaysChart(7)
  } else if (chartMode.value === '30d') {
    renderLastDaysChart(30)
  } else if (chartMode.value === 'year') {
    renderYearChart()
  }
}

const renderLastDaysChart = (days) => {
  if (dailyChartInstance) {
    dailyChartInstance.destroy()
  }

  const today = new Date()
  const labels = []
  const data = []

  for (let i = days - 1; i >= 0; i--) {
    const d = new Date(today)
    d.setDate(today.getDate() - i)

    labels.push(`${d.getMonth() + 1}/${d.getDate()}`)

    const count = allForms.value.filter(f => {
      const created = new Date(f.createdAt)
      return (
          created.getFullYear() === d.getFullYear() &&
          created.getMonth() === d.getMonth() &&
          created.getDate() === d.getDate()
      )
    }).length

    data.push(count)
  }

  dailyChartInstance = new Chart(dailyChart.value, {
    type: 'line',
    data: {
      labels,
      datasets: [
        {
          label: '新增表單數',
          data,
          borderColor: '#3182ce',
          backgroundColor: 'rgba(49,130,206,0.2)',
          tension: 0.3,
          fill: true
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0
        }
      }
    }
    }
  })
}

const renderYearChart = () => {
  if (dailyChartInstance) {
    dailyChartInstance.destroy()
  }

  const now = new Date()
  const year = now.getFullYear()

  const labels = []
  const data = []

  for (let month = 0; month < 12; month++) {
    labels.push(`${month + 1} 月`)

    const count = allForms.value.filter(f => {
      const created = new Date(f.createdAt)
      return (
          created.getFullYear() === year &&
          created.getMonth() === month
      )
    }).length

    data.push(count)
  }

  dailyChartInstance = new Chart(dailyChart.value, {
    type: 'line',
    data: {
      labels,
      datasets: [
        {
          label: `${year} 年每月新增表單`,
          data,
          borderColor: '#38a169',
          backgroundColor: 'rgba(56,161,105,0.2)',
          tension: 0.3,
          fill: true
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0 
        }
      }
    }
    }
  })
}
const pagedForms = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  return allForms.value.slice(start, start + pageSize)
})

// ===== utils =====
const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.dashboard-home {
  max-width: 1200px;
}

.page-header {
  margin-bottom: 32px;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
}

.page-description {
  color: #718096;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.stat-card {
  background: white;
  border-radius: 8px;
  padding: 24px;
  display: flex;
  justify-content: space-between;
  border: 1px solid #e2e8f0;
}

.stat-number {
  font-size: 32px;
  font-weight: 700;
}

.recent-section {
  background: white;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.section-header {
  padding: 24px;
  border-bottom: 1px solid #e2e8f0;
}

.recent-forms {
  padding: 24px;
}

.forms-table {
  width: 100%;
}

.table-header,
.table-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr auto;
  gap: 16px;
}

.table-header {
  font-weight: 600;
  border-bottom: 1px solid #e2e8f0;
  padding-bottom: 8px;
}

.table-row {
  padding: 16px 0;
  border-bottom: 1px solid #f7fafc;
}

.view-btn {
  background: #edf2f7;
  padding: 6px 12px;
  border-radius: 4px;
  text-decoration: none;
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: 24px;
}

.pagination button {
  padding: 6px 12px;
  border-radius: 4px;
  border: 1px solid #e2e8f0;
  background: white;
  cursor: pointer;
}

.pagination button.active {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.pagination button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.chart-tabs {
  display: flex;
  gap: 8px;
}

.chart-tabs button {
  padding: 4px 10px;
  font-size: 12px;
  border-radius: 4px;
  border: 1px solid #e2e8f0;
  background: white;
  cursor: pointer;
}

.chart-tabs button.active {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}
.chart-card canvas {
  width: 100% !important;
  height: 260px !important;
}
</style>
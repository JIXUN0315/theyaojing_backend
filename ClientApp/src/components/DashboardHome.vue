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

    <!-- 最近表單 -->
    <div class="recent-section">
      <div class="section-header">
        <h3 class="section-title">最近提交的表單</h3>
        <router-link to="/dashboard/forms" class="view-all-link">
          查看全部
        </router-link>
      </div>

      <div class="recent-forms">
        <div v-if="loading" class="loading">
          載入中...
        </div>

        <div v-else-if="recentForms.length === 0" class="empty-state">
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
              v-for="form in recentForms"
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
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const loading = ref(true)
const stats = ref({
  totalForms: 0,
  todayForms: 0,
  weekForms: 0,
  monthForms: 0
})

const recentForms = ref([])

onMounted(async () => {
  await loadDashboardData()
})

const loadDashboardData = async () => {
  try {
    loading.value = true

    // 載入表單列表數據
    const response = await fetch('/api/Form')
    if (response.ok) {
      const forms = await response.json()

      // 計算統計數據
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

      // 取最近 5 筆表單
      recentForms.value = forms.slice(0, 5)
    }
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  } finally {
    loading.value = false
  }
}

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
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: 700;
  color: #1a202c;
}

.page-description {
  margin: 0;
  color: #718096;
  font-size: 16px;
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
  align-items: center;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stat-content {
  flex: 1;
}

.stat-number {
  font-size: 32px;
  font-weight: 700;
  color: #2d3748;
  margin-bottom: 4px;
}

.stat-label {
  color: #718096;
  font-size: 14px;
  font-weight: 500;
}

.stat-icon {
  font-size: 32px;
  opacity: 0.7;
}

.recent-section {
  background: white;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px;
  border-bottom: 1px solid #e2e8f0;
}

.section-title {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #2d3748;
}

.view-all-link {
  color: #3182ce;
  text-decoration: none;
  font-weight: 500;
  font-size: 14px;
}

.view-all-link:hover {
  text-decoration: underline;
}

.recent-forms {
  padding: 24px;
}

.loading {
  text-align: center;
  padding: 40px;
  color: #718096;
}

.empty-state {
  text-align: center;
  padding: 40px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-state p {
  margin: 0;
  color: #718096;
  font-size: 16px;
}

.forms-table {
  width: 100%;
}

.table-header {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr auto;
  gap: 16px;
  padding: 12px 0;
  border-bottom: 1px solid #e2e8f0;
  margin-bottom: 8px;
}

.header-cell {
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
}

.table-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr auto;
  gap: 16px;
  padding: 16px 0;
  border-bottom: 1px solid #f7fafc;
  align-items: center;
}

.table-row:last-child {
  border-bottom: none;
}

.table-cell {
  color: #2d3748;
  font-size: 14px;
}

.view-btn {
  background: #edf2f7;
  color: #4a5568;
  padding: 6px 12px;
  border-radius: 4px;
  text-decoration: none;
  font-size: 12px;
  font-weight: 500;
  transition: all 0.2s ease;
}

.view-btn:hover {
  background: #e2e8f0;
  color: #2d3748;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .table-header,
  .table-row {
    grid-template-columns: 1fr 120px;
    gap: 12px;
  }

  .table-header .header-cell:nth-child(2),
  .table-header .header-cell:nth-child(3),
  .table-row .table-cell:nth-child(2),
  .table-row .table-cell:nth-child(3) {
    display: none;
  }
}

@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
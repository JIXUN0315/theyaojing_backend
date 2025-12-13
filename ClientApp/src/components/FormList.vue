<template>
  <div class="form-list">
    <!-- 頁面標題 -->
    <div class="page-header">
      <h2 class="page-title">表單管理</h2>
      <p class="page-description">管理所有使用者提交的表單</p>
    </div>

    <!-- 搜尋和篩選 -->
    <div class="filters">
      <div class="search-box">
        <input
            v-model="searchQuery"
            type="text"`
            placeholder="搜尋姓名或 Email..."
            class="search-input"
        />
      </div>
      <div class="actions">
        <button @click="refreshData" class="refresh-btn" :disabled="loading">
          🔄 重新整理
        </button>
      </div>
    </div>

    <!-- 表單列表 -->
    <div class="table-container">
      <div v-if="loading" class="loading">
        載入中...
      </div>

      <div v-else-if="filteredForms.length === 0" class="empty-state">
        <div class="empty-icon">📋</div>
        <p v-if="searchQuery">找不到符合條件的表單</p>
        <p v-else>暫無表單資料</p>
      </div>

      <div v-else class="table">
        <div class="table-header">
          <div class="header-cell">姓名</div>
          <div class="header-cell">Email</div>
          <div class="header-cell">提交時間</div>
          <div class="header-cell">操作</div>
        </div>

        <div
            v-for="form in paginatedForms"
            :key="form.id"
            class="table-row"
        >
          <div class="table-cell">
            <div class="cell-content">
              <strong>{{ form.fullName }}</strong>
            </div>
          </div>
          <div class="table-cell">
            <div class="cell-content">{{ form.email }}</div>
          </div>
          <div class="table-cell">
            <div class="cell-content">{{ formatDate(form.createdAt) }}</div>
          </div>
          <div class="table-cell">
            <div class="cell-content">
              <router-link
                  :to="`/dashboard/forms/${form.id}`"
                  class="view-btn"
              >
                查看詳情
              </router-link>
            </div>
          </div>
        </div>
      </div>

      <!-- 分頁 -->
      <div v-if="totalPages > 1" class="pagination">
        <button
            @click="goToPage(currentPage - 1)"
            :disabled="currentPage === 1"
            class="page-btn"
        >
          上一頁
        </button>

        <div class="page-info">
          第 {{ currentPage }} 頁，共 {{ totalPages }} 頁
        </div>

        <button
            @click="goToPage(currentPage + 1)"
            :disabled="currentPage === totalPages"
            class="page-btn"
        >
          下一頁
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const loading = ref(true)
const forms = ref([])
const searchQuery = ref('')
const currentPage = ref(1)
const pageSize = 10

onMounted(async () => {
  await loadForms()
})

const loadForms = async () => {
  try {
    loading.value = true
    const response = await fetch('/api/Form')
    if (response.ok) {
      forms.value = await response.json()
    } else {
      console.error('Failed to load forms')
    }
  } catch (error) {
    console.error('Error loading forms:', error)
  } finally {
    loading.value = false
  }
}

const filteredForms = computed(() => {
  if (!searchQuery.value) return forms.value

  const query = searchQuery.value.toLowerCase()
  return forms.value.filter(form =>
      form.fullName.toLowerCase().includes(query) ||
      form.email.toLowerCase().includes(query)
  )
})

const totalPages = computed(() =>
    Math.ceil(filteredForms.value.length / pageSize)
)

const paginatedForms = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  const end = start + pageSize
  return filteredForms.value.slice(start, end)
})

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

const refreshData = async () => {
  await loadForms()
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

// 當搜尋條件改變時重置到第一頁
computed(() => {
  currentPage.value = 1
  return searchQuery.value
})
</script>

<style scoped>
.form-list {
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

.filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  gap: 16px;
}

.search-box {
  flex: 1;
  max-width: 400px;
}

.search-input {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  transition: border-color 0.2s ease;
}

.search-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.actions {
  display: flex;
  gap: 12px;
}

.refresh-btn {
  background: white;
  border: 1px solid #e2e8f0;
  color: #4a5568;
  padding: 10px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
}

.refresh-btn:hover:not(:disabled) {
  background: #f7fafc;
  border-color: #cbd5e0;
}

.refresh-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.table-container {
  background: white;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

.loading {
  text-align: center;
  padding: 60px;
  color: #718096;
  font-size: 16px;
}

.empty-state {
  text-align: center;
  padding: 60px;
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-state p {
  margin: 0;
  color: #718096;
  font-size: 16px;
}

.table {
  width: 100%;
}

.table-header {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr auto;
  gap: 16px;
  padding: 16px 24px;
  background: #f7fafc;
  border-bottom: 1px solid #e2e8f0;
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
  padding: 16px 24px;
  border-bottom: 1px solid #f7fafc;
  transition: background-color 0.2s ease;
}

.table-row:hover {
  background: #f8fafc;
}

.table-row:last-child {
  border-bottom: none;
}

.cell-content {
  color: #2d3748;
  font-size: 14px;
  word-break: break-word;
}

.view-btn {
  background: #3182ce;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
  text-decoration: none;
  font-size: 12px;
  font-weight: 500;
  transition: all 0.2s ease;
  display: inline-block;
}

.view-btn:hover {
  background: #2c5aa0;
  transform: translateY(-1px);
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  background: #f7fafc;
  border-top: 1px solid #e2e8f0;
}

.page-btn {
  background: white;
  border: 1px solid #e2e8f0;
  color: #4a5568;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
}

.page-btn:hover:not(:disabled) {
  background: #f7fafc;
  border-color: #cbd5e0;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  color: #718096;
  font-size: 14px;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .filters {
    flex-direction: column;
    align-items: stretch;
  }

  .search-box {
    max-width: none;
  }

  .table-header,
  .table-row {
    grid-template-columns: 1fr auto;
    gap: 12px;
  }

  .table-header .header-cell:nth-child(2),
  .table-header .header-cell:nth-child(3),
  .table-row .cell-content:nth-child(2),
  .table-row .cell-content:nth-child(3) {
    display: none;
  }

  .pagination {
    flex-direction: column;
    gap: 12px;
  }
}
</style>
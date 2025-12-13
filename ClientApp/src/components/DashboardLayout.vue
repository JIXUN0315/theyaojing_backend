<template>
  <div class="dashboard-layout">
    <!-- 頂部導航列 -->
    <header class="header">
      <div class="header-content">
        <div class="header-left">
          <h1 class="logo">後台管理系統</h1>
        </div>
        <div class="header-right">
          <div class="user-info">
            <span class="username">{{ user?.username || 'Admin' }}</span>
            <button @click="handleLogout" class="logout-btn">
              登出
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- 主要內容區域 -->
    <div class="main-container">
      <!-- 側邊欄 -->
      <aside class="sidebar">
        <nav class="nav-menu">
          <router-link
              to="/dashboard"
              class="nav-item"
              :class="{ active: $route.path === '/dashboard' }"
          >
            <span class="nav-icon">📊</span>
            <span class="nav-text">儀表板</span>
          </router-link>
          <router-link
              to="/dashboard/blog"
              class="nav-item"
              :class="{ active: $route.path.startsWith('/dashboard/blog') }"
          >
            <span class="nav-icon">✏️</span>
            <span class="nav-text">留學部落格</span>
          </router-link>
          <router-link
              to="/dashboard/news"
              class="nav-item"
              :class="{ active: $route.path.startsWith('/dashboard/news') }"
          >
            <span class="nav-icon">📢</span>
            <span class="nav-text">最新消息</span>
          </router-link>
        </nav>
      </aside>

      <!-- 內容區域 -->
      <main class="content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const user = ref(null)

onMounted(() => {
  // 這裡可以從 localStorage 或其他地方取得使用者資訊
  const userInfo = localStorage.getItem('adminUser')
  if (userInfo) {
    user.value = JSON.parse(userInfo)
  }
})

const handleLogout = () => {
  // 1️⃣ 清除 reactive 狀態（最重要）
  user.value = null

  // 2️⃣ 清除 localStorage
  localStorage.removeItem('authToken')

  // 3️⃣ 導向登入頁
  router.replace('/') // 比 push 好
}
</script>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
  background-color: #f8fafc;
}

.header {
  background: white;
  border-bottom: 1px solid #e2e8f0;
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  height: 64px;
}

.logo {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #1a202c;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

.username {
  color: #4a5568;
  font-weight: 500;
}

.logout-btn {
  background: #f7fafc;
  border: 1px solid #e2e8f0;
  color: #4a5568;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
}

.logout-btn:hover {
  background: #edf2f7;
  border-color: #cbd5e0;
}

.main-container {
  display: flex;
  min-height: calc(100vh - 64px);
}

.sidebar {
  width: 240px;
  background: white;
  border-right: 1px solid #e2e8f0;
  padding: 24px 0;
}

.nav-menu {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 24px;
  color: #4a5568;
  text-decoration: none;
  transition: all 0.2s ease;
  border-right: 3px solid transparent;
}

.nav-item:hover {
  background: #f7fafc;
  color: #2d3748;
}

.nav-item.active {
  background: #edf2f7;
  color: #2b6cb0;
  border-right-color: #3182ce;
}

.nav-icon {
  font-size: 18px;
  width: 20px;
  text-align: center;
}

.nav-text {
  font-weight: 500;
}

.content {
  flex: 1;
  padding: 24px;
  background: #f8fafc;
  overflow-y: auto;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .sidebar {
    width: 200px;
  }

  .header-content {
    padding: 0 16px;
  }

  .content {
    padding: 16px;
  }
}

@media (max-width: 480px) {
  .main-container {
    flex-direction: column;
  }

  .sidebar {
    width: 100%;
    border-right: none;
    border-bottom: 1px solid #e2e8f0;
  }

  .nav-menu {
    flex-direction: row;
    overflow-x: auto;
    padding: 0 16px;
  }

  .nav-item {
    flex-shrink: 0;
    border-right: none;
    border-bottom: 3px solid transparent;
  }

  .nav-item.active {
    border-right: none;
    border-bottom-color: #3182ce;
  }
}
</style>
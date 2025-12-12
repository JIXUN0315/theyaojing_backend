<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h2>登入系統</h2>
        <p>請輸入您的帳號密碼</p>
      </div>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username">帳號</label>
          <input
              id="username"
              v-model="form.username"
              type="text"
              placeholder="請輸入帳號"
              required
              :class="{ error: errors.username }"
          />
          <span v-if="errors.username" class="error-message">{{ errors.username }}</span>
        </div>

        <div class="form-group">
          <label for="password">密碼</label>
          <div class="password-input">
            <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="請輸入密碼"
                required
                :class="{ error: errors.password }"
            />
            <button
                type="button"
                class="password-toggle"
                @click="showPassword = !showPassword"
            >
              {{ showPassword ? '👁️' : '🙈' }}
            </button>
          </div>
          <span v-if="errors.password" class="error-message">{{ errors.password }}</span>
        </div>

        <button type="submit" class="login-button" :disabled="isLoading">
          <span v-if="isLoading" class="loading-spinner"></span>
          {{ isLoading ? '登入中...' : '登入' }}
        </button>

        <div v-if="loginError" class="login-error">
          {{ loginError }}
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api/axios.js'   // <-- 使用 axios instance

const router = useRouter()

// 表單資料
const form = reactive({
  username: '',
  password: '',
})

// 狀態
const showPassword = ref(false)
const isLoading = ref(false)
const loginError = ref('')
const errors = reactive({
  username: '',
  password: ''
})

// 表單驗證
const validateForm = () => {
  errors.username = ''
  errors.password = ''

  if (!form.username.trim()) {
    errors.username = '請輸入帳號'
    return false
  }

  if (!form.password.trim()) {
    errors.password = '請輸入密碼'
    return false
  }

  return true
}

// 處理登入
const handleLogin = async () => {
  if (!validateForm()) return

  isLoading.value = true
  loginError.value = ''

  try {

    const response = await api.post('/api/Auth/login', {
      username: form.username,
      password: form.password
    })

    const result = response.data

    if (result.success) {
      // 儲存 Token 與 Admin 資料
      localStorage.setItem('authToken', result.token)
      localStorage.setItem('adminUser', JSON.stringify(result.user))

      // 跳轉到 Dashboard
      router.push('/dashboard')
    } else {
      loginError.value = result.message || '登入失敗'
    }

  } catch (error) {
    console.error(error)
    loginError.value = '登入時發生錯誤，請稍後再試'
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* 🔥 你的原始 CSS 完整保留，不動 */
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  background: white;
  border-radius: 16px;
  padding: 40px;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-header {
  text-align: center;
  margin-bottom: 32px;
}

.login-header h2 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: 700;
  color: #2d3748;
}

.login-header p {
  margin: 0;
  color: #718096;
  font-size: 14px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
}

.form-group input {
  padding: 12px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.2s ease;
  background: #f7fafc;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  background: white;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-group input.error {
  border-color: #e53e3e;
  background: #fed7d7;
}

.password-input {
  position: relative;
  display: flex;
  align-items: center;
}

.password-input input {
  flex: 1;
  padding-right: 50px;
}

.password-toggle {
  position: absolute;
  right: 12px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  padding: 4px;
  border-radius: 4px;
  transition: background-color 0.2s ease;
}

.password-toggle:hover {
  background-color: #edf2f7;
}

.error-message {
  color: #e53e3e;
  font-size: 12px;
  margin-top: 4px;
}

.login-button {
  padding: 14px 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-top: 8px;
}

.login-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.3);
}

.login-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.login-error {
  background: #fed7d7;
  color: #c53030;
  padding: 12px;
  border-radius: 8px;
  text-align: center;
  font-size: 14px;
  margin-top: 8px;
}
</style>

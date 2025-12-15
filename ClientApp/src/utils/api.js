// src/utils/api.js
import axios from "axios"

// 建立 axios instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL, // ✔ 自動使用環境變數
  headers: {
    "Content-Type": "application/json"
  }
})

// 自動附上 Authorization Token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("authToken")
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// 統一處理錯誤（例如 token 過期自動登出）
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.warn("Token expired, redirecting to login")

      localStorage.removeItem("authToken")
      localStorage.removeItem("adminUser")

      if (window.location.pathname !== "/") {
        window.location.href = "/"
      }
    }

    return Promise.reject(error)
  }
)

// ----------- 封裝便利方法 GET / POST / PUT / DELETE -----------

export async function apiGet(url) {
  const res = await api.get(url)
  return res.data
}

export async function apiPost(url, data) {
  const res = await api.post(url, data)
  return res.data
}

export async function apiPut(url, data) {
  const res = await api.put(url, data)
  return res.data
}

export async function apiDelete(url) {
  const res = await api.delete(url)
  return res.data
}

// ----------- Token 驗證工具 -----------

export async function verifyToken() {
  try {
    const res = await api.get("/api/Auth/verify")
    return res.status === 200
  } catch {
    return false
  }
}

export function isAuthenticated() {
  return !!(localStorage.getItem("authToken") && localStorage.getItem("adminUser"))
}

export default api

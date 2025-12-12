import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  base: '/', // 部署在根目錄
  build: {
    emptyOutDir: true
  },
  server: {
     proxy: {
    '/api': {
      target: 'https://localhost:52930',
      changeOrigin: true,
      secure: false   // 因為是本機自簽名 SSL
    }
  }
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  }
})
import react from '@vitejs/plugin-react'
import { defineConfig, loadEnv } from 'vite'

export default defineConfig(({ mode }) => ({
  plugins: [react()],
  server: {
    host: '127.0.0.1',
    proxy: {
      '/api': {
        target: loadEnv(mode, process.cwd(), 'API_').API_BASE_URL || 'http://localhost:5111',
        changeOrigin: true,
      },
    },
  },
}))

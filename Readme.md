# 🐘 PostgreSQL 開發資料庫說明

本專案使用 Docker 建立本地開發用的 PostgreSQL 資料庫，並自動匯入初始化表單資料表（`form_responses`）。

---

## 🛠 環境需求

- [Docker](https://www.docker.com/) / Docker Desktop
- 可選：`psql` CLI 工具（若需手動測試）

---

## 🚀 快速啟動指令

```bash
docker compose down -v  # 清除舊資料
docker compose up --build

建立 .env 檔案放在根目錄 將 寄送 email相關資訊放置於此 
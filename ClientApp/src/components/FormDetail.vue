<template>
  <div class="form-detail">
    <!-- 載入狀態 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>載入中...</p>
    </div>

    <!-- 錯誤狀態 -->
    <div v-else-if="error" class="error-container">
      <div class="error-icon">❌</div>
      <h3>載入失敗</h3>
      <p>{{ error }}</p>
      <div class="error-actions">
        <button @click="loadForm" class="retry-btn">重新載入</button>
        <router-link to="/dashboard/forms" class="back-btn"
          >返回列表</router-link
        >
      </div>
    </div>

    <!-- 表單詳情 -->
    <div v-else-if="form" class="form-content">
      <!-- 頁面標題和操作 -->
      <div class="page-header">
        <div class="header-left">
          <router-link to="/dashboard" class="back-link">
            ← 返回列表
          </router-link>
          <h2 class="page-title">表單詳情</h2>
        </div>
      </div>

      <!-- 基本資訊卡片 -->
      <div class="info-card">
        <div class="card-header">
          <h3>基本資訊</h3>
          <div class="timestamp">
            提交時間：{{ formatDate(form.createdAt) }}
          </div>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <label class="info-label">姓名</label>
            <div class="info-value">{{ form.fullName }}</div>
          </div>

          <div class="info-item">
            <label class="info-label">Email</label>
            <div class="info-value">
              <a :href="`mailto:${form.email}`" class="email-link">
                {{ form.email }}
              </a>
            </div>
          </div>

          <div class="info-item">
            <label class="info-label">聯絡方式</label>
            <div class="info-value">{{ form.phoneOrLine || "未提供" }}</div>
          </div>
        </div>
      </div>

      <!-- 學歷資訊卡片 -->
      <div class="info-card">
        <div class="card-header">
          <h3>學歷資訊</h3>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <label class="info-label">學校</label>
            <div class="info-value">{{ form.school || "未提供" }}</div>
          </div>

          <div class="info-item">
            <label class="info-label">科系</label>
            <div class="info-value">{{ form.department || "未提供" }}</div>
          </div>
        </div>
      </div>

      <!-- 留學資訊卡片 -->
      <div class="info-card">
        <div class="card-header">
          <h3>留學資訊</h3>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <label class="info-label">目標國家</label>
            <div class="info-value">
              <span class="country-tag">{{
                form.targetCountry || "未提供"
              }}</span>
            </div>
          </div>

          <div class="info-item">
            <label class="info-label">課程類型</label>
            <div class="info-value">{{ form.programType || "未提供" }}</div>
          </div>

          <div class="info-item">
            <label class="info-label">希望專業</label>
            <div class="info-value">{{ form.intendedMajor || "未提供" }}</div>
          </div>

          <div class="info-item">
            <label class="info-label">預計出發年份</label>
            <div class="info-value">{{ form.departYear || "未提供" }}</div>
          </div>
        </div>
      </div>

      <!-- 其他資訊卡片 -->
      <div class="info-card">
        <div class="card-header">
          <h3>其他資訊</h3>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <label class="info-label">得知來源</label>
            <div class="info-value">{{ form.referral || "未提供" }}</div>
          </div>
           <div class="info-item">
            <label class="info-label">諮詢方式</label>
            <div class="info-value">{{ form.askType || "未提供" }}</div>
          </div>
           <div class="info-item">
            <label class="info-label">最想解決的問題</label>
            <div class="info-value">{{ form.questionToResolve || "未提供" }}</div>
          </div>

          <div class="info-item full-width" v-if="form.otherInfo">
            <label class="info-label">補充說明</label>
            <div class="info-value other-info">
              {{ form.otherInfo }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { apiGet } from "../utils/api.js";

const route = useRoute();
const router = useRouter();

const loading = ref(true);
const error = ref("");
const form = ref(null);
const isDeleting = ref(false);

onMounted(async () => {
  await loadForm();
});

const loadForm = async () => {
  try {
    loading.value = true;
    error.value = "";

    const formId = route.params.id;
    form.value = await apiGet(`/api/Form/${formId}`);
  } catch (err) {
    error.value = err.message || "載入表單資料時發生錯誤";
    console.error("Failed to load form:", err);
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleString("zh-TW", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });
};

const exportForm = () => {
  if (!form.value) return;

  // 建立要匯出的資料
  const exportData = {
    姓名: form.value.fullName,
    Email: form.value.email,
    聯絡方式: form.value.phoneOrLine || "未提供",
    學校: form.value.school || "未提供",
    科系: form.value.department || "未提供",
    目標國家: form.value.targetCountry || "未提供",
    課程類型: form.value.programType || "未提供",
    希望專業: form.value.intendedMajor || "未提供",
    預計出發年份: form.value.departYear || "未提供",
    得知來源: form.value.referral || "未提供",
    補充說明: form.value.otherInfo || "未提供",
    提交時間: formatDate(form.value.createdAt),
  };

  // 轉換為 JSON 字串並建立下載
  const dataStr = JSON.stringify(exportData, null, 2);
  const dataBlob = new Blob([dataStr], { type: "application/json" });

  const url = URL.createObjectURL(dataBlob);
  const link = document.createElement("a");
  link.href = url;
  link.download = `表單_${form.value.fullName}_${
    new Date().toISOString().split("T")[0]
  }.json`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
};

const deleteForm = async () => {
  if (!form.value || isDeleting.value) return;

  const confirmed = confirm(
    `確定要刪除 ${form.value.fullName} 的表單嗎？此操作無法復原。`
  );
  if (!confirmed) return;

  try {
    isDeleting.value = true;

    // 這裡需要實作刪除 API
    // await apiDelete(`/api/Form/${route.params.id}`)

    // 暫時模擬刪除成功
    alert("表單已刪除");
    router.push("/dashboard/forms");
  } catch (err) {
    alert("刪除失敗：" + (err.message || "未知錯誤"));
    console.error("Delete failed:", err);
  } finally {
    isDeleting.value = false;
  }
};
</script>

<style scoped>
.form-detail {
  max-width: 1000px;
  margin: 0 auto;
}

.loading-container {
  text-align: center;
  padding: 80px 20px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e2e8f0;
  border-top: 3px solid #3182ce;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-container {
  text-align: center;
  padding: 80px 20px;
}

.error-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.error-container h3 {
  margin: 0 0 8px 0;
  color: #e53e3e;
  font-size: 24px;
}

.error-container p {
  margin: 0 0 24px 0;
  color: #718096;
}

.error-actions {
  display: flex;
  justify-content: center;
  gap: 12px;
}

.retry-btn,
.back-btn {
  padding: 10px 20px;
  border-radius: 6px;
  text-decoration: none;
  display: inline-block;
  font-weight: 500;
  transition: all 0.2s ease;
}

.retry-btn {
  background: #3182ce;
  color: white;
  border: none;
  cursor: pointer;
}

.retry-btn:hover {
  background: #2c5aa0;
}

.back-btn {
  background: #f7fafc;
  color: #4a5568;
  border: 1px solid #e2e8f0;
}

.back-btn:hover {
  background: #edf2f7;
}

.form-content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
}

.header-left {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.back-link {
  color: #3182ce;
  text-decoration: none;
  font-weight: 500;
  font-size: 14px;
  transition: color 0.2s ease;
}

.back-link:hover {
  color: #2c5aa0;
  text-decoration: underline;
}

.page-title {
  margin: 0;
  font-size: 28px;
  font-weight: 700;
  color: #1a202c;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.export-btn,
.delete-btn {
  padding: 10px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid;
}

.export-btn {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.export-btn:hover {
  background: #2c5aa0;
  border-color: #2c5aa0;
}

.delete-btn {
  background: #fff;
  color: #e53e3e;
  border-color: #e53e3e;
}

.delete-btn:hover:not(:disabled) {
  background: #e53e3e;
  color: white;
}

.delete-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.info-card {
  background: white;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

.card-header {
  padding: 20px 24px 16px;
  border-bottom: 1px solid #f7fafc;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #2d3748;
}

.timestamp {
  color: #718096;
  font-size: 14px;
}

.info-grid {
  padding: 24px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-item.full-width {
  grid-column: 1 / -1;
}

.info-label {
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-value {
  color: #2d3748;
  font-size: 16px;
  line-height: 1.5;
  min-height: 24px;
}

.email-link {
  color: #3182ce;
  text-decoration: none;
}

.email-link:hover {
  text-decoration: underline;
}

.country-tag {
  background: #edf2f7;
  color: #4a5568;
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 14px;
  font-weight: 500;
}

.other-info {
  background: #f7fafc;
  padding: 16px;
  border-radius: 6px;
  border-left: 4px solid #3182ce;
  white-space: pre-wrap;
  word-break: break-word;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .header-actions {
    justify-content: stretch;
  }

  .export-btn,
  .delete-btn {
    flex: 1;
  }

  .info-grid {
    grid-template-columns: 1fr;
    padding: 16px;
  }

  .card-header {
    padding: 16px;
  }

  .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
}

@media (max-width: 480px) {
  .form-detail {
    margin: 0 -8px;
  }

  .info-card {
    border-radius: 0;
    border-left: none;
    border-right: none;
  }
}
</style>

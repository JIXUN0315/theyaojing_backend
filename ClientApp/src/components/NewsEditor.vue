<template>
  <div class="news-editor">
    <!-- 頁面標題 -->
    <div class="page-header">
      <div class="header-left">
        <router-link to="/dashboard/news" class="back-link">
          ← 返回最新消息列表
        </router-link>
        <h2 class="page-title">{{ isEditing ? "編輯消息" : "發布新消息" }}</h2>
      </div>
      <div class="header-actions">
        <button
          @click="edit(false)"
          class="save-draft-btn"
          :disabled="isSaving || !canSave"
        >
          💾 {{ isSaving ? "儲存中..." : "儲存草稿" }}
        </button>
        <button @click="edit(true)" class="publish-btn" :disabled="isSaving || !canSave">
          📢 {{ isSaving ? "發布中..." : "立即發布" }}
        </button>
      </div>
    </div>

    <!-- 編輯表單 -->
    <div class="editor-container">
      <div class="editor-main">
        <!-- 消息標題 -->
        <div class="form-group">
          <label class="form-label">消息標題 *</label>
          <input
            v-model="news.title"
            type="text"
            placeholder="輸入消息標題..."
            class="title-input"
            maxlength="200"
          />
          <div class="char-count">{{ news.title.length }}/200</div>
        </div>
        <div class="form-group">
          <label class="form-label">發布時間</label>

          <VueDatePicker
  v-model="news.date"
  :time-config="{ enableTimePicker: false }"
  :formats="{ input: 'yyyy-MM-dd' }"
  :day-names="['日', '一', '二', '三', '四', '五', '六']"
  placeholder="請選擇日期"
  :clearable="false"
  :auto-apply="true"
/>
        </div>
        <!-- 消息圖片 -->
        <div class="form-group">
          <label class="form-label">消息圖片</label>
          <div class="image-upload">
            <div v-if="news.imgUrl" class="image-preview">
              <img :src="news.imgUrl" alt="消息圖片預覽" />
              <button @click="removeImage" class="remove-image-btn">✕</button>
            </div>
            <div v-else class="image-placeholder">
              <input
                ref="imageInput"
                type="file"
                accept="image/*"
                @change="handleImageUpload"
                class="image-input"
              />
              <div class="upload-area" @click="$refs.imageInput.click()">
                <div class="upload-icon">📷</div>
                <p>點擊上傳圖片</p>
              </div>
            </div>
          </div>
        </div>

        <!-- 內容編輯器 -->
        <div class="form-group">
          <label class="form-label">消息內容 *</label>
          <div class="form-group">
            <div ref="editorRef"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { VueDatePicker } from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import { useRoute, useRouter } from "vue-router";
import { apiPost, apiGet } from "../utils/api.js";
import suneditor from "suneditor";
import plugins from "suneditor/src/plugins";
import "suneditor/dist/css/suneditor.min.css";
import Swal from "sweetalert2";
const route = useRoute();
const router = useRouter();

const isEditing = computed(() => route.params.id !== undefined);
const isSaving = ref(false);

const news = ref({
  title: "",
  content: "",
  imgUrl: "",
  isPublished: false,
  date: dayjs().hour(12).toDate(),
});

const editorRef = ref(null);
let sunEditorInstance = null;
const canSave = computed(() => {
  return (
    news.value.title.trim() !== "" &&
    news.value.content.trim() !== "" &&
    news.value.imgUrl.trim() !== "" &&
    news.value.date !== ""
  );
});
onMounted(async () => {
  // 先初始化 editor（一定要傳 plugins，fontSize 才存在）
  sunEditorInstance = suneditor.create(editorRef.value, {
    plugins, // ✅ 這行是解決 callPlugin.fail 的關鍵
    showPathLabel: false,
    width: "100%",
    height: "400px",
    defaultStyle:
      "font-family: 'Noto Sans TC', 'Microsoft JhengHei', Arial; font-size: 16px; line-height: 1.7;",

    font: [
      "Noto Sans TC", // ✅ 正確（Web）
      "Microsoft JhengHei",
      "Arial",
      "Tahoma",
      "Verdana",
    ],
    fontSize: [12, 14, 16, 18, 20, 24, 28, 32],
    formats: ["p", "div", "blockquote", "h1", "h2", "h3", "h4", "h5", "h6"],
    buttonList: [
      ["undo", "redo"],
      ["font", "fontSize", "formatBlock"],
      ["bold", "underline", "italic", "strike"],
      ["fontColor", "hiliteColor"],
      ["align", "list", "lineHeight"],
       ["link"],  
      ["removeFormat"],
    ],
  });

  // 初始內容
  sunEditorInstance.setContents(news.value.content || "");

  // 同步內容回 Vue state
  sunEditorInstance.onChange = (content) => {
    news.value.content = content;
  };

  // 如果是編輯模式，載入資料後再 setContents（很重要）
  if (isEditing.value) {
    await loadNews();
    sunEditorInstance.setContents(news.value.content || "");
  }
});

onBeforeUnmount(() => {
  if (sunEditorInstance) {
    sunEditorInstance.destroy();
    sunEditorInstance = null;
  }
});
async function loadNews() {
  const id = route.params.id;
  if (!id) return;

  try {
    const res = await apiGet(`/api/news/${id}`);

    // ⚠️ 關鍵：逐欄位 assign，不要整包覆蓋 ref
    news.value.title = res.title || "";
    news.value.content = res.content || "";
    news.value.imgUrl = res.imgUrl || "";
    news.value.isPublished = res.isPublished ?? false;
    news.value.date = res.date ?? null;
  } catch (err) {
    console.error(err);
    await Swal.fire("載入失敗", "無法取得文章資料", "error");
    router.push("/dashboard/news");
  }
}
const handleImageUpload = async (event) => {
  const file = event.target.files?.[0];
  if (!file) return;
  if (!file.type.startsWith("image/")) {
     await Swal.fire("載入失敗", "請選擇圖片檔案", "error");
    return;
  }

  try {
    isSaving.value = true;
    const folder = "news";
    const signData = await apiGet(
      `/api/Cloudinary/sign?folder=${encodeURIComponent(folder)}`
    );
    const formData = new FormData();
    formData.append("file", file);
    formData.append("api_key", signData.apiKey);
    formData.append("timestamp", signData.timestamp);
    formData.append("signature", signData.signature);
    formData.append("folder", folder);
    const uploadRes = await fetch(
      `https://api.cloudinary.com/v1_1/${signData.cloudName}/image/upload`,
      { method: "POST", body: formData }
    );

    const uploadData = await uploadRes.json();
    if (!uploadRes.ok) {
      console.error("Cloudinary error:", uploadData);
      throw new Error(uploadData?.error?.message || "Cloudinary upload failed");
    }
    news.value.imgUrl = uploadData.secure_url;
    event.target.value = "";
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "儲存失敗",
      text: "圖片上傳失敗，請稍後再試",
    });
  } finally {
    isSaving.value = false;
  }
};
const removeImage = () => {
  news.value.imgUrl = "";
};
const edit = async (isPublished) => {
  isSaving.value = true;
  if (isEditing.value) {
    await updateNews(isPublished);
  } else {
    await createNews(isPublished);
  }
  isSaving.value = false;
   Swal.fire({
      toast: true,
      icon: "success",
      title: "編輯完成",
      timer: 1500,
      showConfirmButton: false,
      position: "top-end",
    }).then(() => {
      router.push("/dashboard/blog");
    });
  router.push("/dashboard/news");
};
const createNews = async (isPublished) => {
  await apiPost("/api/news/create", {
    title: news.value.title,
    content: news.value.content,
    imgUrl: news.value.imgUrl,
    date: news.value.date,
    isPublished: isPublished,
  });
};
const updateNews = async (isPublished) => {
  await apiPost("/api/news/update", {
    id: route.params.id,
    title: news.value.title,
    content: news.value.content,
    imgUrl: news.value.imgUrl,
    date: news.value.date,
    isPublished: isPublished,
  });
};
</script>

<style scoped>
/* 全域設定 box-sizing */
* {
  box-sizing: border-box;
}

.news-editor {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
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

.save-draft-btn,
.publish-btn {
  padding: 12px 20px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  border: 1px solid;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 8px;
  box-sizing: border-box;
}

.save-draft-btn {
  background: white;
  color: #4a5568;
  border-color: #e2e8f0;
}

.save-draft-btn:hover:not(:disabled) {
  background: #f7fafc;
  border-color: #cbd5e0;
}

.publish-btn {
  background: #38a169;
  color: white;
  border-color: #38a169;
}

.publish-btn:hover:not(:disabled) {
  background: #2f855a;
  transform: translateY(-1px);
}

.save-draft-btn:disabled,
.publish-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.editor-container {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 32px;
}

.editor-main {
  background: white;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  padding: 24px;
  box-sizing: border-box;
}

.form-group {
  margin-bottom: 24px;
}

.form-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
}

/* 修正輸入框超出問題 */
.title-input {
  width: 100%;
  padding: 16px 20px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 20px;
  font-weight: 600;
  transition: all 0.2s ease;
  box-sizing: border-box;
}

.title-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.char-count {
  text-align: right;
  font-size: 12px;
  color: #718096;
  margin-top: 4px;
}

.form-hint {
  font-size: 12px;
  color: #718096;
  margin-top: 4px;
}

.image-upload {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.image-preview {
  position: relative;
  max-width: 400px;
}

.image-preview img {
  width: 100%;
  height: auto;
  border-radius: 8px;
}

.remove-image-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  border: none;
  border-radius: 50%;
  width: 28px;
  height: 28px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.2s ease;
}

.remove-image-btn:hover {
  background: rgba(0, 0, 0, 0.9);
}

.image-placeholder {
  position: relative;
}

.image-input {
  display: none;
}

.upload-area {
  border: 2px dashed #cbd5e0;
  border-radius: 8px;
  padding: 40px;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s ease;
  box-sizing: border-box;
}

.upload-area:hover {
  border-color: #3182ce;
  background: #f7fafc;
}

.upload-icon {
  font-size: 32px;
  margin-bottom: 12px;
}

.upload-area p {
  margin: 0;
  color: #718096;
  font-size: 14px;
}

.image-url-input {
  width: 100%;
  padding: 10px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  box-sizing: border-box;
}

.image-url-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.editor-wrapper {
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  overflow: hidden;
  transition: border-color 0.2s ease;
  box-sizing: border-box;
}

.editor-wrapper:focus-within {
  border-color: #3182ce;
}

.editor-toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
  flex-wrap: wrap;
  box-sizing: border-box;
}

.toolbar-group {
  display: flex;
  gap: 4px;
  padding-right: 8px;
  border-right: 1px solid #e2e8f0;
}

.toolbar-group:last-child {
  border-right: none;
}

.toolbar-btn {
  padding: 8px 10px;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s ease;
  min-width: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
}

.toolbar-btn:hover {
  background: #edf2f7;
  border-color: #cbd5e0;
}

.toolbar-btn.active {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.format-select {
  padding: 8px 10px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 13px;
  background: white;
  box-sizing: border-box;
}

.rich-editor {
  min-height: 400px;
  padding: 20px;
  outline: none;
  line-height: 1.7;
  font-size: 15px;
  width: 100%;
  box-sizing: border-box;
  overflow-wrap: break-word;
}

.rich-editor:focus {
  background: #fafafa;
}

.html-textarea {
  width: 100%;
  min-height: 400px;
  padding: 20px;
  border: none;
  font-family: "Courier New", monospace;
  font-size: 14px;
  resize: vertical;
  outline: none;
  box-sizing: border-box;
}

.html-textarea:focus {
  background: #fafafa;
}

.editor-sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.sidebar-section {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 20px;
  box-sizing: border-box;
}

.sidebar-title {
  margin: 0 0 16px 0;
  font-size: 16px;
  font-weight: 600;
  color: #2d3748;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  font-size: 14px;
  color: #4a5568;
  font-weight: 500;
}

.checkmark {
  width: 16px;
  height: 16px;
  border: 2px solid #e2e8f0;
  border-radius: 4px;
  position: relative;
}

.datetime-input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  box-sizing: border-box;
}

.datetime-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.stats-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  font-size: 13px;
  color: #718096;
}

.stat-value {
  font-size: 13px;
  font-weight: 600;
  color: #2d3748;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.preview-btn,
.duplicate-btn,
.delete-btn {
  width: 100%;
  padding: 12px 16px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  box-sizing: border-box;
}

.preview-btn {
  background: #edf2f7;
  color: #4a5568;
  border-color: #e2e8f0;
}

.preview-btn:hover {
  background: #e2e8f0;
}

.duplicate-btn {
  background: #e6fffa;
  color: #319795;
  border-color: #81e6d9;
}

.duplicate-btn:hover {
  background: #b2f5ea;
}

.delete-btn {
  background: white;
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

/* 響應式設計 */
@media (max-width: 1024px) {
  .editor-container {
    grid-template-columns: 1fr;
  }

  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .header-actions {
    justify-content: stretch;
  }

  .save-draft-btn,
  .publish-btn {
    flex: 1;
  }
}

@media (max-width: 768px) {
  .news-editor {
    padding: 0 10px;
  }

  .editor-toolbar {
    padding: 8px;
  }

  .toolbar-group {
    padding-right: 4px;
  }

  .rich-editor {
    min-height: 300px;
    padding: 16px;
  }

  .html-textarea {
    min-height: 300px;
    padding: 16px;
  }

  .editor-main {
    padding: 16px;
  }

  .sidebar-section {
    padding: 16px;
  }
}
.none .dp--tp-wrap{

  display: none !important;
}
</style>

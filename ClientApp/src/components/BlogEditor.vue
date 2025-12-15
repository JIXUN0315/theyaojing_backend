<template>
  <div class="blog-editor">
    <!-- 頁面標題 -->
    <div class="page-header">
      <div class="header-left">
        <router-link to="/dashboard/blog" class="back-link">
          ← 返回部落格列表
        </router-link>
        <h2 class="page-title">{{ isEditing ? "編輯文章" : "新增文章" }}</h2>
      </div>
      <div class="header-actions">
        <button @click="saveDraft" class="save-draft-btn" :disabled="isSaving">
          💾 {{ isSaving ? "儲存中..." : "儲存草稿" }}
        </button>
        <button
          @click="publishArticle"
          class="publish-btn"
          :disabled="isSaving"
        >
          🚀 {{ isSaving ? "發布中..." : "發布文章" }}
        </button>
      </div>
    </div>

    <!-- 編輯表單 -->
    <div class="editor-container">
      <div class="editor-main">
        <!-- 標題 -->
        <div class="form-group">
          <label class="form-label">文章標題 *</label>
          <input
            v-model="article.title"
            type="text"
            placeholder="輸入文章標題..."
            class="title-input"
            maxlength="50"
          />
          <div class="char-count">{{ article.title.length }}/100</div>
        </div>
        <div class="form-group">
          <label class="form-label">學校/副標題1</label>
          <input
            v-model="article.subtitle1"
            type="text"
            placeholder="輸入學校/副標題..."
            class="title-input"
            maxlength="50"
          />
          <div class="char-count">{{ article.subtitle1.length }}/100</div>
        </div>
        <div class="form-group">
          <label class="form-label">學位學程/副標題2</label>
          <input
            v-model="article.subtitle2"
            type="text"
            placeholder="輸入學位學程/副標題..."
            class="title-input"
            maxlength="50"
          />
          <div class="char-count">{{ article.subtitle2.length }}/100</div>
        </div>
        <div class="form-group">
          <label class="form-label">發布時間 *</label>

          <VueDatePicker
            v-model="article.date"
            :time-config="{ enableTimePicker: false }"
            :formats="{ input: 'yyyy-MM-dd' }"
            :day-names="['日', '一', '二', '三', '四', '五', '六']"
            placeholder="請選擇日期"
            :clearable="false"
            :auto-apply="true"
          />
        </div>
        <!-- 文章分類 -->
        <div class="form-group">
          <label class="form-label">文章分類 *</label>

          <div class="radio-group">
            <!-- 既有分類 -->
            <label
              class="radio-option"
              v-for="option in categoryOptions"
              :key="option"
            >
              <input
                type="radio"
                name="category"
                :value="option"
                v-model="selectedCategory"
              />
              <span class="radio-label">{{ option }}</span>
            </label>

            <!-- 自訂分類 -->
            <label class="radio-option">
              <input
                type="radio"
                name="category"
                value="__custom__"
                v-model="selectedCategory"
              />
              <span class="radio-label">自行輸入</span>
            </label>
          </div>

          <!-- 自訂輸入框 -->
          <div
            class="custom-category-input"
            v-if="selectedCategory === '__custom__'"
          >
            <span class="hash-prefix">#</span>
            <input
              v-model="customCategory"
              type="text"
              placeholder="請輸入分類名稱"
              class="title-input"
              maxlength="200"
            />
          </div>

          <div class="form-hint">選擇或輸入最符合文章內容的分類</div>
        </div>
        <!-- 精選分享 -->
        <div class="form-group">
          <label class="form-label">精選分享</label>

          <label class="checkbox-option">
            <input type="checkbox" v-model="article.isFeatured" />
            <span class="checkbox-label">設為精選分享</span>
          </label>

          <div class="form-hint">
            勾選後，文章會顯示於首頁「學生回饋精選」區塊
          </div>
        </div>
        <!-- 特色圖片 -->
        <div class="form-group">
          <label class="form-label">封面圖片 *</label>
          <div class="image-upload">
            <div v-if="article.coverImage" class="image-preview">
              <img :src="article.coverImage" alt="特色圖片預覽" />
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
              </div>
            </div>
          </div>
        </div>
        <!-- 文章圖片（多張） -->
        <div class="form-group">
          <label class="form-label">文章圖片（可上傳多張）</label>

          <div class="multi-image-upload">
            <!-- 已上傳圖片預覽 -->
            <div
              class="multi-image-preview"
              v-for="(img, index) in article.images"
              :key="index"
            >
              <img :src="img.url" alt="文章圖片預覽" />
              <button
                class="remove-image-btn"
                @click="removeContentImage(index)"
              >
                ✕
              </button>
            </div>

            <!-- 上傳按鈕 -->
            <div class="image-placeholder">
              <input
                ref="contentImageInput"
                type="file"
                accept="image/*"
                multiple
                @change="handleContentImagesUpload"
                class="image-input"
              />
              <div class="upload-area" @click="$refs.contentImageInput.click()">
                <div class="upload-icon">➕</div>
                <div class="upload-text">新增圖片</div>
              </div>
            </div>
          </div>

          <div class="form-hint">圖片將依上傳順序顯示於文章中</div>
        </div>

        <!-- 內容編輯器 -->
        <div class="form-group">
          <label class="form-label">文章內容 *</label>
          <div class="form-group">
            <div ref="editorRef"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { apiGet, apiPost, apiPut } from "../utils/api.js";
import { uploadImageToCloudinary } from "../utils/cloudinary.js";
import Swal from "sweetalert2";
import { VueDatePicker } from "@vuepic/vue-datepicker";
import suneditor from "suneditor";
import plugins from "suneditor/src/plugins";
import "suneditor/dist/css/suneditor.min.css";
import dayjs from "dayjs";
const route = useRoute();
const router = useRouter();

const isEditing = computed(() => route.params.id !== undefined);
const isSaving = ref(false);
const categoryDropdownOpen = ref(false);
// radio 選到的值
const selectedCategory = ref("");
const editorRef = ref(null);
// 自訂分類（不含 #）
const customCategory = ref("");
const categoryOptions = ["#留學故事", "#服務心得", "#實用專欄"];

const article = ref({
  id: null,
  title: "",
  subtitle1: "",
  subtitle2: "",
  content: "",
  category: "",
  coverImage: "", // 對應後端 CoverImage
  images: [], // 對應後端 Images
  isPublished: false,
  isFeatured: false,
  date: dayjs().hour(12).toDate(),
  slug: "",
});

const editor = ref(null);
let sunEditorInstance = null;
onMounted(async () => {
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
  sunEditorInstance.setContents(article.value.content || "");

  // 同步內容回 Vue state
  sunEditorInstance.onChange = (content) => {
    article.value.content = content;
  };
  if (isEditing.value) {
    await loadArticle();
    sunEditorInstance.setContents(article.value.content || "");
    const matched = categoryOptions.includes(article.category);
    if (matched) {
      selectedCategory.value = article.category;
    } else {
      selectedCategory.value = "__custom__";
      customCategory.value = article.category.replace(/^#/, "");
    }
  }

  if (editor.value) {
    editor.value.innerHTML = article.value.content || "";
  }

  // 添加點擊外部關閉下拉選單的事件監聽
  const handleClickOutside = (e) => {
    if (!e.target.closest(".custom-select")) {
      categoryDropdownOpen.value = false;
    }
  };
  document.addEventListener("click", handleClickOutside);
});
// 同步回 article.category（關鍵）
watch([selectedCategory, customCategory], () => {
  if (selectedCategory.value === "__custom__") {
    article.category = customCategory.value ? `#${customCategory.value}` : "";
  } else {
    article.category = selectedCategory.value;
  }
});
const loadArticle = async () => {
  try {
    const data = await apiGet(`/api/Blog/${route.params.id}`);

    article.value = {
      id: data.id,
      title: data.title ?? "",
      subtitle1: data.subtitle1 ?? "",
      subtitle2: data.subtitle2 ?? "",
      content: data.content ?? "",
      category: data.category ?? "",
      coverImage: data.coverImage ?? "",
      images: data.images ?? [],
      isPublished: data.isPublished ?? false,
      isFeatured: data.isFeatured ?? false,
      date: data.date ? new Date(data.date) : new Date(),
      slug: data.slug ?? "",
    };

    // ⭐ 分類同步到 radio
    const matched = categoryOptions.includes(article.value.category);
    if (matched) {
      selectedCategory.value = article.value.category;
      customCategory.value = "";
    } else {
      selectedCategory.value = "__custom__";
      customCategory.value = article.value.category.replace(/^#/, "");
    }

    // ⭐ SunEditor 內容
    sunEditorInstance.setContents(article.value.content || "");
  } catch (error) {
    console.error("Failed to load article:", error);
    await Swal.fire("載入失敗", "無法取得文章資料", "error");
  }
};

const handleImageUpload = async (event) => {
  const file = event.target.files[0];
  if (!file) return;
  try {
    const imageUrl = await uploadImageToCloudinary(file, "blog_featured");
    article.value.coverImage = imageUrl;
  } catch (err) {
    await Swal.fire({
      icon: "error",
      title: "儲存失敗",
      text: "圖片上傳失敗，請稍後再試",
    });
  }
};

const removeImage = () => {
  article.value.coverImage = "";
};
const handleContentImagesUpload = async (e) => {
  const files = Array.from(e.target.files);
  for (const file of files) {
    const url = await uploadImageToCloudinary(file, "blog_content");
    article.value.images.push({
      url,
      sort: article.value.images.length,
    });
  }
};
const removeContentImage = (index) => {
  article.value.images.splice(index, 1);
};
const saveDraft = async () => {
  const errorMessage = validateArticleRequiredFields();
  if (errorMessage) {
    await Swal.fire("無法儲存草稿", errorMessage, "warning");
    return;
  }
  try {
    isSaving.value = true;
    article.value.isPublished = false;
    await saveArticle();
    Swal.fire({
      toast: true,
      icon: "success",
      title: "草稿已儲存",
      timer: 1500,
      showConfirmButton: false,
      position: "top-end",
    }).then(() => {
      router.push("/dashboard/blog");
    });
  } catch (error) {
    console.log(error);
    await Swal.fire("儲存失敗", "", "error");
  } finally {
    isSaving.value = false;
  }
};

const publishArticle = async () => {
  const errorMessage = validateArticleRequiredFields();
  if (errorMessage) {
    await Swal.fire("無法儲存草稿", errorMessage, "warning");
    return;
  }
  try {
    isSaving.value = true;
    article.value.isPublished = true;
    await saveArticle();
    Swal.fire({
      toast: true,
      icon: "success",
      title: "文章已發布",
      timer: 1500,
      showConfirmButton: false,
      position: "top-end",
    }).then(() => {
      router.push("/dashboard/blog");
    });
  } catch (error) {
    alert("發布失敗");
  } finally {
    isSaving.value = false;
  }
};

const saveArticle = async () => {
  const errorMessage = validateArticleRequiredFields();
  if (errorMessage) {
    await Swal.fire("無法儲存草稿", errorMessage, "warning");
    return;
  }
  const payload = {
    ...article.value,
    imagesJson: JSON.stringify(article.value.images),
  };
  payload.category =
    selectedCategory.value === "__custom__"
      ? customCategory.value
      : selectedCategory.value;
  if (isEditing.value) {
    await apiPut(`/api/Blog/${route.params.id}`, payload);
  } else {
    await apiPost("/api/Blog", payload);
  }
};
const validateArticleRequiredFields = () => {
  if (!article.value.title?.trim()) {
    return "請輸入文章標題";
  }

  if (!article.value.date) {
    return "請選擇發布時間";
  }

  if (!selectedCategory.value?.trim()) {
    return "請選擇文章分類";
  }

  if (!article.value.coverImage?.trim()) {
    return "請上傳封面圖片";
  }

  // 去除 HTML tag 後再判斷內容
  const plainText = article.value.content?.replace(/<[^>]*>/g, "").trim();

  if (!plainText) {
    return "請輸入文章內容";
  }

  return null; // ✅ 通過
};
</script>

<style scoped>
/* 全域設定 box-sizing */
* {
  box-sizing: border-box;
}

.blog-editor {
  max-width: 1400px;
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
}

.back-link:hover {
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
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid;
  transition: all 0.2s ease;
  box-sizing: border-box;
}

.save-draft-btn {
  background: white;
  color: #4a5568;
  border-color: #e2e8f0;
}

.save-draft-btn:hover:not(:disabled) {
  background: #f7fafc;
}

.publish-btn {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.publish-btn:hover:not(:disabled) {
  background: #2c5aa0;
}

.save-draft-btn:disabled,
.publish-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.editor-container {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 32px;
}

.editor-main {
  background: white;
  border-radius: 8px;
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
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 18px;
  font-weight: 600;
  box-sizing: border-box;
}

.title-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.slug-input-group {
  display: flex;
  align-items: center;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  overflow: hidden;
  width: 100%;
  box-sizing: border-box;
}

.slug-prefix {
  background: #f7fafc;
  padding: 12px 16px;
  color: #718096;
  font-size: 14px;
  border-right: 1px solid #e2e8f0;
  flex-shrink: 0;
}

.slug-input {
  flex: 1;
  padding: 12px 16px;
  border: none;
  font-size: 14px;
  min-width: 0; /* 防止 flex item 超出 */
  box-sizing: border-box;
}

.slug-input:focus {
  outline: none;
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

.custom-select {
  position: relative;
  width: 100%;
}

.select-trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 16px 20px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
  cursor: pointer;
  transition: all 0.3s ease;
  box-sizing: border-box;
  min-height: 56px;
}

.select-trigger:hover {
  border-color: #cbd5e0;
  background: white;
}

.select-trigger.has-value {
  color: #2d3748;
  font-weight: 500;
}

.custom-select.open .select-trigger {
  border-color: #3182ce;
  box-shadow: 0 0 0 4px rgba(49, 130, 206, 0.15);
  background: white;
  border-bottom-left-radius: 4px;
  border-bottom-right-radius: 4px;
}

.select-value {
  font-size: 15px;
  color: #2d3748;
  flex: 1;
}

.select-trigger:not(.has-value) .select-value {
  color: #a0aec0;
}

.select-arrow {
  color: #718096;
  transition: all 0.2s ease;
  margin-left: 12px;
}

.custom-select.open .select-arrow {
  color: #3182ce;
  transform: rotate(180deg);
}

.select-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 2px solid #3182ce;
  border-top: none;
  border-radius: 0 0 12px 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1),
    0 2px 4px -1px rgba(0, 0, 0, 0.06);
  z-index: 1000;
  max-height: 300px;
  overflow-y: auto;
}

.select-option {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  cursor: pointer;
  transition: all 0.2s ease;
  border-bottom: 1px solid #f7fafc;
}

.select-option:last-child {
  border-bottom: none;
}

.select-option:hover {
  background: #f7fafc;
}

.select-option.active {
  background: #ebf8ff;
  color: #3182ce;
  font-weight: 600;
}

.option-icon {
  font-size: 18px;
  margin-right: 12px;
}

.option-text {
  flex: 1;
  font-size: 15px;
}

.option-check {
  color: #3182ce;
  font-weight: bold;
  font-size: 16px;
}

.image-upload {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.image-preview {
  position: relative;
  max-width: 300px;
}

.image-preview img {
  width: 100%;
  height: auto;
  border-radius: 6px;
}

.remove-image-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  cursor: pointer;
}

.image-placeholder {
  position: relative;
}

.image-input {
  display: none;
}

.upload-area {
  border: 2px dashed #e2e8f0;
  border-radius: 6px;
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
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  overflow: hidden;
  box-sizing: border-box;
}

.editor-toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #f7fafc;
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
  padding: 6px 8px;
  border: 1px solid #e2e8f0;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: all 0.2s ease;
  box-sizing: border-box;
}

.toolbar-btn:hover {
  background: #edf2f7;
}

.toolbar-btn.active {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.format-select {
  padding: 6px 8px;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 12px;
  box-sizing: border-box;
}

.rich-editor {
  min-height: 400px;
  padding: 16px;
  outline: none;
  line-height: 1.6;
  width: 100%;
  box-sizing: border-box;
  overflow-wrap: break-word;
}

.rich-editor:focus {
  background: #f8fafc;
}

.html-textarea {
  width: 100%;
  min-height: 400px;
  padding: 16px;
  border: none;
  font-family: "Courier New", monospace;
  font-size: 14px;
  resize: vertical;
  box-sizing: border-box;
}

.html-textarea:focus {
  outline: none;
  background: #f8fafc;
}

.editor-sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.sidebar-section {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
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
  gap: 8px;
  cursor: pointer;
  font-size: 14px;
  color: #4a5568;
}

.checkbox-label input[type="checkbox"] {
  margin: 0;
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
  font-size: 14px;
  color: #718096;
}

.stat-value {
  font-size: 14px;
  font-weight: 600;
  color: #2d3748;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.preview-btn,
.delete-btn {
  width: 100%;
  padding: 10px 16px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  border: 1px solid;
  transition: all 0.2s ease;
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
.radio-group {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 8px;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}

.custom-category-input {
  display: flex;
  align-items: center;
  margin-top: 8px;
}

.hash-prefix {
  padding: 13.5px 13px;
  background: #f2f2f2;
  border: 1px solid #ccc;
  border-right: none;
  border-radius: 4px 0 0 4px;
  position: relative;
  left: 5px;
}

.custom-category-input .text-input {
  flex: 1;
  border-radius: 0 4px 4px 0;
}
.multi-image-upload {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.multi-image-preview {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  max-width: 163px;
  max-height: 163px;
  border-radius: 6px;
  padding: 6px;
  background: #fafafa;
}

.multi-image-preview img {
  max-width: 163px;
  max-height: 163px;
  width: auto;
  height: auto;
  object-fit: contain;
}

.multi-image-preview .remove-image-btn {
  position: absolute;
  top: 4px;
  right: 4px;
}
.checkbox-option {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
.checkbox-label {
  font-size: 16px;
}
.checkbox-option input {
  width: 16px;
  height: 16px;
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
  .editor-toolbar {
    padding: 8px;
  }

  .toolbar-group {
    padding-right: 4px;
  }

  .rich-editor {
    min-height: 300px;
    padding: 12px;
  }

  .html-textarea {
    min-height: 300px;
    padding: 12px;
  }

  .editor-main {
    padding: 16px;
  }

  .sidebar-section {
    padding: 16px;
  }
}
</style>

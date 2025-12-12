<template>
  <div class="blog-editor">
    <!-- 頁面標題 -->
    <div class="page-header">
      <div class="header-left">
        <router-link to="/dashboard/blog" class="back-link">
          ← 返回部落格列表
        </router-link>
        <h2 class="page-title">{{ isEditing ? '編輯文章' : '新增文章' }}</h2>
      </div>
      <div class="header-actions">
        <button @click="saveDraft" class="save-draft-btn" :disabled="isSaving">
          💾 {{ isSaving ? '儲存中...' : '儲存草稿' }}
        </button>
        <button @click="publishArticle" class="publish-btn" :disabled="isSaving">
          🚀 {{ isSaving ? '發布中...' : '發布文章' }}
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
              maxlength="200"
          />
          <div class="char-count">{{ article.title.length }}/200</div>
        </div>

        <!-- 摘要 -->
        <div class="form-group">
          <label class="form-label">文章摘要</label>
          <textarea
              v-model="article.excerpt"
              placeholder="輸入文章摘要，將顯示在列表和分享時..."
              class="excerpt-input"
              rows="3"
              maxlength="300"
          ></textarea>
          <div class="char-count">{{ article.excerpt.length }}/300</div>
        </div>

        <!-- 文章分類 -->
        <div class="form-group">
          <label class="form-label">文章分類 *</label>
          <div class="custom-select" :class="{ open: categoryDropdownOpen }">
            <div
                class="select-trigger"
                @click="toggleCategoryDropdown"
                :class="{ 'has-value': article.category }"
            >
              <span class="select-value">
                {{ article.category ? getCategoryText(article.category) : '請選擇文章分類' }}
              </span>
              <div class="select-arrow">
                <svg width="12" height="8" viewBox="0 0 12 8" fill="none">
                  <path d="M1 1.5L6 6.5L11 1.5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </div>
            </div>
            <div class="select-dropdown" v-show="categoryDropdownOpen">
              <div
                  class="select-option"
                  v-for="option in categoryOptions"
                  :key="option.value"
                  @click="selectCategory(option.value)"
                  :class="{ active: article.category === option.value }"
              >
                <span class="option-icon">{{ option.icon }}</span>
                <span class="option-text">{{ option.label }}</span>
                <span v-if="article.category === option.value" class="option-check">✓</span>
              </div>
            </div>
          </div>
          <div class="form-hint">選擇最符合文章內容的分類</div>
        </div>

        <!-- 特色圖片 -->
        <div class="form-group">
          <label class="form-label">特色圖片</label>
          <div class="image-upload">
            <div v-if="article.featuredImageUrl" class="image-preview">
              <img :src="article.featuredImageUrl" alt="特色圖片預覽" />
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
                <p>點擊上傳圖片或輸入圖片網址</p>
              </div>
            </div>
            <input
                v-model="article.featuredImageUrl"
                type="url"
                placeholder="或輸入圖片網址..."
                class="image-url-input"
            />
          </div>
        </div>

        <!-- 文字編輯器 -->
        <div class="form-group">
          <label class="form-label">文章內容 *</label>
          <div class="editor-wrapper">
            <div class="editor-toolbar">
              <div class="toolbar-group">
                <button @click="execCommand('bold')" class="toolbar-btn" title="粗體">
                  <strong>B</strong>
                </button>
                <button @click="execCommand('italic')" class="toolbar-btn" title="斜體">
                  <em>I</em>
                </button>
                <button @click="execCommand('underline')" class="toolbar-btn" title="底線">
                  <u>U</u>
                </button>
              </div>

              <div class="toolbar-group">
                <select @change="execCommand('formatBlock', $event.target.value)" class="format-select">
                  <option value="">格式</option>
                  <option value="h1">標題 1</option>
                  <option value="h2">標題 2</option>
                  <option value="h3">標題 3</option>
                  <option value="p">段落</option>
                </select>
              </div>

              <div class="toolbar-group">
                <button @click="execCommand('insertUnorderedList')" class="toolbar-btn" title="項目符號">
                  •
                </button>
                <button @click="execCommand('insertOrderedList')" class="toolbar-btn" title="編號">
                  1.
                </button>
              </div>
              

              <div class="toolbar-group">
                <button @click="toggleHtmlMode" class="toolbar-btn" :class="{ active: showHtml }" title="HTML 模式">
                  &lt;/&gt;
                </button>
              </div>
            </div>

            <div v-if="!showHtml" class="editor-content">
              <div
                  ref="editor"
                  class="rich-editor"
                  contenteditable="true"
                  @input="updateContent"
                  @paste="handlePaste"
              />
            </div>

            <div v-else class="html-editor">
              <textarea
                  v-model="article.content"
                  class="html-textarea"
                  placeholder="輸入 HTML 內容..."
              ></textarea>
            </div>
          </div>
        </div>
      </div>

      <!-- 側邊欄設定 -->
      <div class="editor-sidebar">
        <div class="sidebar-section">
          <h3 class="sidebar-title">統計資訊</h3>
          <div class="stats-info">
            <div class="stat-item">
              <span class="stat-label">字數統計</span>
              <span class="stat-value">{{ wordCount }}</span>
            </div>
          </div>
        </div>
        
      </div>
    </div>
  </div>
</template>

<script setup>
import {computed, onMounted, ref} from 'vue'
import {useRoute, useRouter} from 'vue-router'
import {apiGet, apiPost, apiPut} from '../utils/api.js'
import { uploadImageToCloudinary } from '../utils/cloudinary.js'
import Swal from 'sweetalert2'


const route = useRoute()
const router = useRouter()

const isEditing = computed(() => route.params.id !== undefined)
const isSaving = ref(false)
const isDeleting = ref(false)
const showHtml = ref(false)
const categoryDropdownOpen = ref(false)

const categoryOptions = [
  { value: 'things-to-know', label: '留學後才知道的事', icon: '📚' },
  { value: 'how-i-changed', label: '留學後的我變了', icon: '🌟' },
  { value: 'career-sharing', label: '國外職業分享', icon: '💼' },
  { value: 'comfort-zone', label: '勇敢跳出舒適圈：以前的我 vs 現在的我', icon: '🚀' },
  { value: 'student-choice', label: '學生自選主題', icon: '✨' }
]

const article = ref({
  title: '',
  slug: '',
  content: '',
  excerpt: '',
  category: '',
  featuredImageUrl: '',
  isPublished: false,
  isFeatured: false,
})

const editor = ref(null)
const imageInput = ref(null)

onMounted(async () => {
  if (isEditing.value) {
    await loadArticle()
  }

  if (editor.value) {
    editor.value.innerHTML = article.value.content || ''
  }
  

  // 添加點擊外部關閉下拉選單的事件監聽
  const handleClickOutside = (e) => {
    if (!e.target.closest('.custom-select')) {
      categoryDropdownOpen.value = false
    }
  }
  document.addEventListener('click', handleClickOutside)
})


const loadArticle = async () => {
  try {
    article.value = await apiGet(`/api/Blog/${route.params.id}`)
  
  } catch (error) {
    console.error('Failed to load article:', error)
  }
}

const wordCount = computed(() => {
  const text = article.value.content.replace(/<[^>]*>/g, '')
  return text.length
})

const updateContent = () => {
  if (editor.value) {
    article.value.content = editor.value.innerHTML
  }
}

const execCommand = (command, value = null) => {
  document.execCommand(command, false, value)
  updateContent()
}

const insertLink = () => {
  const url = prompt('請輸入連結網址:')
  if (url) {
    execCommand('createLink', url)
  }
}

const insertImage = () => {
  const url = prompt('請輸入圖片網址:')
  if (url) {
    execCommand('insertImage', url)
  }
}

const toggleHtmlMode = () => {
  showHtml.value = !showHtml.value
}

const handlePaste = (event) => {
  event.preventDefault()
  const text = event.clipboardData.getData('text/plain')
  document.execCommand('insertText', false, text)
}

const handleImageUpload = async (event) => {
  const file = event.target.files[0]
  if(!file) return
  try {
    const imageUrl = await uploadImageToCloudinary(file, 'blog_featured');
    article.value.featuredImageUrl = imageUrl;
  } catch (err) {
    await Swal.fire({
      icon: 'error',
      title: '儲存失敗',
      text: '圖片上傳失敗，請稍後再試'
    })
  }
  
}

const removeImage = () => {
  article.value.featuredImageUrl = ''
}

const saveDraft = async () => {
  try {
    isSaving.value = true
    article.value.isPublished = false
    await saveArticle()
    Swal.fire({
      toast: true,
      icon: 'success',
      title: '草稿已儲存',
      timer: 1500,
      showConfirmButton: false,
      position: 'top-end'
    }).then(() => {
      router.push('/dashboard/blog')
    });

  } catch (error) {
    console.log(error)
    alert('儲存失敗')
  } finally {
    isSaving.value = false
  }
}

const publishArticle = async () => {
  if (!article.value.title.trim()) {
    alert('請輸入文章標題')
    return
  }

  if (!article.value.category) {
    alert('請選擇文章分類')
    return
  }

  if (!article.value.content.trim()) {
    alert('請輸入文章內容')
    return
  }

  try {
    isSaving.value = true
    article.value.isPublished = true
    await saveArticle()
    Swal.fire({
      toast: true,
      icon: 'success',
      title: '文章已發布',
      timer: 1500,
      showConfirmButton: false,
      position: 'top-end'
    }).then(() => {
      router.push('/dashboard/blog')
    });
  } catch (error) {

    alert('發布失敗')
  } finally {
    isSaving.value = false
  }
}

const saveArticle = async () => {
  // 自動產生 slug
  if (!article.value.slug && article.value.title) {
    article.value.slug = article.value.title
        .toLowerCase()
        .replace(/[^a-z0-9\s-]/g, '')
        .replace(/\s+/g, '-')
  }
  const isEdit = route.params.id !== undefined;

  if (isEdit) {
    // 呼叫 PUT 更新
    await apiPut(`/api/Blog/${route.params.id}`, article.value);
  } else {
    // 呼叫 POST 新增
    await apiPost('/api/Blog', article.value);
  }
}


const getCategoryText = (categoryValue) => {
  const option = categoryOptions.find(opt => opt.value === categoryValue)
  return option ? `${option.icon} ${option.label}` : '未分類'
}

const toggleCategoryDropdown = () => {
  categoryDropdownOpen.value = !categoryDropdownOpen.value
}

const selectCategory = (value) => {
  article.value.category = value
  categoryDropdownOpen.value = false
}

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

.save-draft-btn, .publish-btn {
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

.save-draft-btn:disabled, .publish-btn:disabled {
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

.excerpt-input {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  resize: vertical;
  box-sizing: border-box;
}

.excerpt-input:focus {
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
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
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
  font-family: 'Courier New', monospace;
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

.preview-btn, .delete-btn {
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

  .save-draft-btn, .publish-btn {
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
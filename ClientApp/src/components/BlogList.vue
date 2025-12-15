<template>
  <div class="blog-list">
    <!-- 頁面標題 -->
    <div class="page-header">
      <div class="header-left">
        <h2 class="page-title">留學部落格管理</h2>
        <p class="page-description">管理部落格文章和內容</p>
      </div>
      <div class="header-actions">
        <router-link to="/dashboard/blog/create" class="create-btn">
          ✏️ 新增文章
        </router-link>
      </div>
    </div>

    <!-- 篩選和搜尋 -->
    <div class="filters">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="搜尋文章標題..."
          class="search-input"
        />
      </div>
      <div class="filter-buttons">
        <button
          @click="statusFilter = 'all'"
          :class="{ active: statusFilter === 'all' }"
          class="filter-btn"
        >
          全部
        </button>
        <button
          @click="statusFilter = 'published'"
          :class="{ active: statusFilter === 'published' }"
          class="filter-btn"
        >
          已發布
        </button>
        <button
          @click="statusFilter = 'draft'"
          :class="{ active: statusFilter === 'draft' }"
          class="filter-btn"
        >
          草稿
        </button>
      </div>
      <div class="actions">
        <button @click="refreshData" class="refresh-btn" :disabled="loading">
          🔄 重新整理
        </button>
      </div>
    </div>

    <!-- 文章列表 -->
    <div class="articles-container">
      <div v-if="loading" class="loading">載入中...</div>

      <div v-else-if="filteredArticles.length === 0" class="empty-state">
        <div class="empty-icon">📝</div>
        <p v-if="searchQuery">找不到符合條件的文章</p>
        <p v-else>
          暫無文章，<router-link to="/dashboard/blog/create"
            >立即新增第一篇文章</router-link
          >
        </p>
      </div>

      <div v-else class="articles-grid">
        <div
          v-for="article in paginatedArticles"
          :key="article.id"
          class="article-card"
        >
          <div class="article-image">
            <img
              :src="
                article.coverImage ||
                'https://res.cloudinary.com/drmlihopq/image/upload/v1765616589/news/oca3vnj789e6nlnzfq7q.png'
              "
              :alt="article.title"
              @error="handleImageError"
            />
            <div class="article-status">
              <span v-if="article.isPublished" class="status-badge published"
                >已發布</span
              >
              <span v-else class="status-badge draft">草稿</span>
              <span v-if="article.isFeatured" class="status-badge featured"
                >精選</span
              >
            </div>
          </div>

          <div class="article-content">
            <h3 class="article-title">{{ article.title }}</h3>

            <div class="article-meta">
              <div class="meta-info">
                <span class="date">{{ formatDate(article.date) }}</span>
              </div>

              <div class="article-actions">
                <router-link
                  :to="`/dashboard/blog/edit/${article.id}`"
                  class="action-btn edit"
                >
                  編輯
                </router-link>
                <button
                  @click="togglePublish(article)"
                  class="action-btn toggle"
                  :disabled="article.updating"
                >
                  {{ article.isPublished ? "取消發布" : "發布" }}
                </button>
                <button
                  @click="deleteArticle(article)"
                  class="action-btn delete"
                  :disabled="article.deleting"
                >
                  {{ article.deleting ? "刪除中..." : "刪除" }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 分頁 -->
      <div v-if="totalPages > 1" class="pagination">
        <button
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="page-btn"
        >
          上一頁
        </button>

        <div class="page-info">
          第 {{ currentPage }} 頁，共 {{ totalPages }} 頁
        </div>

        <button
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="page-btn"
        >
          下一頁
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { apiDelete, apiGet, apiPost } from "../utils/api.js";

const loading = ref(true);
const articles = ref([]);
const searchQuery = ref("");
const statusFilter = ref("all");
const currentPage = ref(1);
const pageSize = 12;

onMounted(async () => {
  await loadArticles();
});

const loadArticles = async () => {
  try {
    loading.value = true;
    // 這裡需要實作後端 API
    articles.value = await apiGet("/api/Blog");
  } catch (error) {
    console.error("Error loading articles:", error);
  } finally {
    loading.value = false;
  }
};

const filteredArticles = computed(() => {
  let filtered = articles.value;

  // 狀態篩選
  if (statusFilter.value === "published") {
    filtered = filtered.filter((article) => article.isPublished);
  } else if (statusFilter.value === "draft") {
    filtered = filtered.filter((article) => !article.isPublished);
  }

  // 搜尋篩選
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      (article) =>
        article.title.toLowerCase().includes(query) ||
        (article.excerpt && article.excerpt.toLowerCase().includes(query))
    );
  }

  return filtered;
});

const totalPages = computed(() =>
  Math.ceil(filteredArticles.value.length / pageSize)
);

const paginatedArticles = computed(() => {
  const start = (currentPage.value - 1) * pageSize;
  const end = start + pageSize;
  return filteredArticles.value.slice(start, end);
});

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

const refreshData = async () => {
  await loadArticles();
};

const togglePublish = async (article) => {
  const nextStatus = !article.isPublished;

  try {
    article.updating = true;

    await apiPost(`/api/blog/${article.id}/publish`, {
      isPublished: nextStatus,
    });
    article.isPublished = nextStatus;
    console.log(
      `${article.isPublished ? "發布" : "取消發布"}文章:`,
      article.title
    );
  } catch (error) {
    console.error("Toggle publish failed:", error);
    alert("操作失敗，請稍後再試");
  } finally {
    article.updating = false;
  }
};

const deleteArticle = async (article) => {
  const confirmed = confirm(
    `確定要刪除文章「${article.title}」嗎？此操作無法復原。`
  );
  if (!confirmed) return;

  try {
    article.deleting = true;
    let res = await apiDelete(`/api/Blog/${article.id}`);
    console.log(res);
    articles.value = articles.value.filter((a) => a.id !== article.id);
  } catch (error) {
    alert("刪除失敗");
  } finally {
    article.deleting = false;
  }
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString("zh-TW", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
};
const handleImageError = (event) => {
  event.target.src =
    "https://res.cloudinary.com/drmlihopq/image/upload/v1765616589/news/oca3vnj789e6nlnzfq7q.png";
};

// 當搜尋條件改變時重置到第一頁
computed(() => {
  currentPage.value = 1;
  return [searchQuery.value, statusFilter.value];
});
</script>

<style scoped>
.blog-list {
  max-width: 1400px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
}

.header-left h2 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: 700;
  color: #1a202c;
}

.header-left p {
  margin: 0;
  color: #718096;
  font-size: 16px;
}

.create-btn {
  background: #3182ce;
  color: white;
  padding: 12px 20px;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.2s ease;
}

.create-btn:hover {
  background: #2c5aa0;
  transform: translateY(-1px);
}

.filters {
  display: flex;
  gap: 16px;
  align-items: center;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.search-box {
  flex: 1;
  max-width: 300px;
}

.search-input {
  width: 100%;
  padding: 10px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
}

.search-input:focus {
  outline: none;
  border-color: #3182ce;
  box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.1);
}

.filter-buttons {
  display: flex;
  gap: 8px;
}

.filter-btn {
  padding: 8px 16px;
  border: 1px solid #e2e8f0;
  background: white;
  color: #4a5568;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
}

.filter-btn:hover {
  border-color: #cbd5e0;
}

.filter-btn.active {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.refresh-btn {
  background: white;
  border: 1px solid #e2e8f0;
  color: #4a5568;
  padding: 10px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
}

.refresh-btn:hover:not(:disabled) {
  background: #f7fafc;
}

.articles-container {
  background: white;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

.loading {
  text-align: center;
  padding: 60px;
  color: #718096;
}

.empty-state {
  text-align: center;
  padding: 60px;
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.empty-state p {
  margin: 0;
  color: #718096;
}

.empty-state a {
  color: #3182ce;
  text-decoration: none;
}

.empty-state a:hover {
  text-decoration: underline;
}

.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 24px;
  padding: 24px;
}

.article-card {
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.2s ease;
}

.article-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.article-image {
  position: relative;
  height: 400px;
  overflow: hidden;
}

.article-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.article-status {
  position: absolute;
  top: 12px;
  right: 12px;
  display: flex;
  gap: 8px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.published {
  background: #c6f6d5;
  color: #22543d;
}

.status-badge.draft {
  background: #fed7cc;
  color: #c53030;
}

.status-badge.featured {
  background: #ffd6cc;
  color: #dd6b20;
}

.article-content {
  padding: 20px;
}

.article-title {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #2d3748;
  line-height: 1.4;
}

.article-excerpt {
  margin: 0 0 16px 0;
  color: #718096;
  font-size: 14px;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
}

.meta-info {
  display: flex;
  gap: 16px;
  font-size: 12px;
  color: #718096;
}

.article-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid;
  transition: all 0.2s ease;
  text-decoration: none;
  display: inline-block;
}

.action-btn.edit {
  background: #edf2f7;
  color: #4a5568;
  border-color: #e2e8f0;
}

.action-btn.edit:hover {
  background: #e2e8f0;
}

.action-btn.toggle {
  background: #3182ce;
  color: white;
  border-color: #3182ce;
}

.action-btn.toggle:hover:not(:disabled) {
  background: #2c5aa0;
}

.action-btn.delete {
  background: white;
  color: #e53e3e;
  border-color: #e53e3e;
}

.action-btn.delete:hover:not(:disabled) {
  background: #e53e3e;
  color: white;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  background: #f7fafc;
  border-top: 1px solid #e2e8f0;
}

.page-btn {
  background: white;
  border: 1px solid #e2e8f0;
  color: #4a5568;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.page-btn:hover:not(:disabled) {
  background: #f7fafc;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  color: #718096;
  font-size: 14px;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .filters {
    flex-direction: column;
    align-items: stretch;
  }

  .search-box {
    max-width: none;
  }

  .articles-grid {
    grid-template-columns: 1fr;
    padding: 16px;
  }

  .article-meta {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .article-actions {
    width: 100%;
    justify-content: space-between;
  }
}
</style>

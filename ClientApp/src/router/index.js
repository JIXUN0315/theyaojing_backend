// src/router/index.js
import { createRouter, createWebHistory } from "vue-router";
import LoginView from "../components/LoginView.vue";
import DashboardLayout from "../components/DashboardLayout.vue";
import DashboardHome from "../components/DashboardHome.vue";
import FormDetail from "../components/FormDetail.vue";
import BlogEditor from "../components/BlogEditor.vue";
import NewsList from "../components/NewsList.vue";
import BlogList from "../components/BlogList.vue";
import NewsEditor from "../components/NewsEditor.vue";

function FormList() {}

const routes = [
  {
    path: "/",
    component: LoginView,
  },
  {
    path: "/dashboard",
    component: DashboardLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: "",
        name: "Dashboard",
        component: DashboardHome,
        meta: { requiresAuth: true },
      },
      {
        path: "forms",
        name: "FormList",
        component: FormList,
        meta: { requiresAuth: true },
      },
      {
        path: "forms/:id",
        name: "FormDetail",
        component: FormDetail,
        meta: { requiresAuth: true },
      },
      {
        path: "blog",
        name: "BlogList",
        component: BlogList,
        meta: { requiresAuth: true },
      },
      {
        path: "blog/create",
        name: "BlogCreate",
        component: BlogEditor,
        meta: { requiresAuth: true },
      },
      {
        path: "blog/edit/:id",
        name: "BlogEdit",
        component: BlogEditor,
        meta: { requiresAuth: true },
      },
      {
        path: "news",
        name: "NewsList",
        component: NewsList,
        meta: { requiresAuth: true },
      },
      {
        path: "news/create",
        name: "NewsCreate",
        component: NewsEditor,
        meta: { requiresAuth: true },
      },
      {
        path: "news/edit/:id",
        name: "NewsEdit",
        component: NewsEditor,
        meta: { requiresAuth: true },
      },
      {
        path: "upload",
        name: "CloudinaryUpload",
        component: () => import("../components/CloudinaryUploadTest.vue"),
        meta: { requiresAuth: true },
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    // 瀏覽器上一頁 / 下一頁
    if (savedPosition) {
      return savedPosition;
    }

    // 進入新頁面 → 回到最上面
    return { top: 0 };
  },
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("authToken");

  // 需要登入的頁面且沒有 token → 導向登入頁
  if (to.meta.requiresAuth && !token) {
    return next("/");
  }

  // 已登入但進入登入頁 → 直接導向 dashboard
  if (to.path === "/" && token) {
    return next("/dashboard");
  }

  next();
});

export default router;

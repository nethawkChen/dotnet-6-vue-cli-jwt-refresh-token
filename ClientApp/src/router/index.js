import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import HeaderView from "../views/HeaderView.vue";
import LoginView from "../views/LoginView.vue";
import TestView from "../views/TestView.vue";

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
    meta: { requireAuth: true },
    children: [
      {
        path: "",
        components: {
          Header: HeaderView
        }
      }
    ]
  },
  {
    path: "/login",
    name: "login",
    component: LoginView
  },
  {
    path: "/about",
    name: "about",
    component: () => import(/* webpackChunkName: "about" */ "../views/AboutView.vue")
  },
  {
    path: "/test",
    name: "test",
    meta: { requireAuth: true },
    component: TestView,
    children: [
      {
        path: "",
        components: {
          Header: HeaderView
        }
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

router.beforeEach(async (to, from) => {
  console.log("to:", to);
  console.log("from:", from);

  if (to.meta.requireAuth) {
    const accessToken = sessionStorage.getItem("accessToken");
    const refreshToken = sessionStorage.getItem("refreshToken");
    console.log("accessToken=", accessToken);
    console.log("refreshToken=", refreshToken);
    if (accessToken === null && refreshToken === null) {
      sessionStorage.setItem("redirectPath", to.fullPath);
      return { name: "login" };
    }
  }
});

export default router;

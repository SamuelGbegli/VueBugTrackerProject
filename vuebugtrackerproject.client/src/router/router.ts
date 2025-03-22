import RegistrationPage from "@/pages/auth/RegistrationPage.vue";
import CreditsPage from "@/pages/CreditsPage.vue";
import MainPage from "@/pages/MainPage.vue";
import MainTemplate from "@/templates/MainTemplate.vue";

import { createRouter, createWebHistory } from "vue-router"

const router = createRouter({
 history: createWebHistory(),
 routes: [
  {
    path: "/",
    component: MainTemplate,
    children:[
      {
        path: "",
        component: MainPage
      },
      {
        path: "register",
        component: RegistrationPage
      },
      {
        path: "credits",
        component: CreditsPage
      }
    ]
  }
 ]
})

export default router;

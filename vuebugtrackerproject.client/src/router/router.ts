import AccountRecoveryPage from "@/pages/auth/AccountRecoveryPage.vue";
import PasswordResetPage from "@/pages/auth/PasswordResetPage.vue";
import RegistrationPage from "@/pages/auth/RegistrationPage.vue";
import CreditsPage from "@/pages/CreditsPage.vue";
import MainPage from "@/pages/MainPage.vue";
import Sandbox from "@/pages/other/sandbox.vue";
import CreateProjectPage from "@/pages/projects/CreateProjectPage.vue";
import ViewProjectPage from "@/pages/projects/ViewProjectPage.vue";
import MainTemplate from "@/templates/MainTemplate.vue";
import ProjectTemplate from "@/templates/ProjectTemplate.vue";

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
        path: "recoveraccount",
        component: AccountRecoveryPage
      },
      {
        path: "resetpassword",
        component: PasswordResetPage
      },
      {
        path: "createproject",
        component: CreateProjectPage
      },
      {
        path: "credits",
        component: CreditsPage
      },
      {
        path: "sandbox",
        component: Sandbox
      },
      {
        path: "project",
        component: ProjectTemplate,
        children:[
          {
            path: ":projectId",
            component: ViewProjectPage
          }
        ]
      }
    ]
  }
 ]
})

export default router;

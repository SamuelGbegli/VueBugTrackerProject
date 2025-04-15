import AccountRecoveryPage from "@/pages/auth/AccountRecoveryPage.vue";
import PasswordResetPage from "@/pages/auth/PasswordResetPage.vue";
import RegistrationPage from "@/pages/auth/RegistrationPage.vue";
import BugDiscussionPage from "@/pages/bugs/BugDiscussionPage.vue";
import BugSettingsPage from "@/pages/bugs/BugSettingsPage.vue";
import ViewBugPage from "@/pages/bugs/ViewBugPage.vue";
import CreditsPage from "@/pages/CreditsPage.vue";
import MainPage from "@/pages/MainPage.vue";
import Sandbox from "@/pages/other/sandbox.vue";
import AddBugPage from "@/pages/projects/AddBugPage.vue";
import CreateProjectPage from "@/pages/projects/CreateProjectPage.vue";
import ProjectBugsPage from "@/pages/projects/ProjectBugsPage.vue";
import ViewProjectPage from "@/pages/projects/ViewProjectPage.vue";
import BugTemplate from "@/templates/BugTemplate.vue";
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
          },
          {
            path: ":projectId/bugs",
            component: ProjectBugsPage
          },
          {
            path: ":projectId/bugs/add",
            component: AddBugPage
          }
        ]
      },
      {
        path: "bug",
        component:BugTemplate,
        children:[
          {
            path: ":bugId",
            component: ViewBugPage
          },
          {
            path: ":bugId/settings",
            component: BugSettingsPage
          },
          {
            path: ":bugId/discussion",
            component: BugDiscussionPage
          }
        ]
      }
    ]
  }
 ]
})

export default router;

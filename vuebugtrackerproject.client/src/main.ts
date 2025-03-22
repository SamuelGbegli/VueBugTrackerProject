import './assets/main.css'
import 'quasar/dist/quasar.css'
import'@quasar/extras/material-icons/material-icons.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router/router'
import {Quasar, Dialog, Loading, Notify} from 'quasar'
import { createPinia } from 'pinia'
import { useAuthStore } from './stores/AuthStore'

const pinia = createPinia();

createApp(App)
.use(router)
.use(Quasar,{
  plugins:{
    Dialog, Loading, Notify
  }
}
)
.use(pinia)
.mount('#app')

//List of routes the user can see only if not logged in
const nonLoggedInOnlyPages = ["/register"];

router.beforeEach((to) =>{


//Checks if the user's JWT is valid every time the page changes
  const authStore = useAuthStore();
  authStore.isJWTValid();

  //Redirects logged in user to main page if they go to pages
  //only non-logged in users can view
  if(authStore.isLoggedIn()){
    if(nonLoggedInOnlyPages.includes(to.fullPath)){
      router.replace({path:"/"});
    }
  }
})

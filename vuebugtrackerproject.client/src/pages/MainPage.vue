<template>
  <h2>Main Page</h2>
  <br/>
  <div>
  <!--Welcome banner. Hidden if user is logged in-->
    <QBanner class="bg-purple-8" v-if="!authStore.isLoggedIn()">
      <h4>Welcome to VueBugTracker</h4>
      <p>Register or log in to access more features.</p>
      <template v-slot:action>
        <!--TODO: add links to registration page-->
        <QBtn @click="showLoginDialog" label="Login"/>
        <QBtn to="register" label="Register"/>
      </template>
    </QBanner>
  </div>
</template>
<script setup lang="ts">

import { Dialog } from 'quasar';
import LoginDialog from '@/dialogs/LoginDialog.vue';
import { useAuthStore } from '@/stores/AuthStore';
import router from '@/router/router';

const authStore = useAuthStore();

//Function to open the login dialog
function showLoginDialog(){
  //Opens dialog
Dialog.create({
  component: LoginDialog,
  componentProps:{

  }
}).onOk(() => {
  console.log("Called OK");
  router.go(0);
}).onCancel(() =>{
  console.log("Called cancel");
}).onDismiss(() => {
  console.log("Called either OK or Cancel");
})
}
</script>

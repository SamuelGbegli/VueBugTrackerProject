
<template>
  <QLayout view="hHh lpR fFf">

    <QHeader reveal bordered class="bg-primary text-white">
      <QToolbar>
        <QToolbarTitle>
          <QAvatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg">
          </QAvatar>
          VueBugTracker
        </QToolbarTitle>
        <QSpace/>
        <div class="q-pa-md q-gutter-md">
          <QBtn to="/" label="Home"/>
        <QBtn to="#" label="Browse"/>
        <!-- section for non logged in users -->
        <QBtn v-if="!authStore.isLoggedIn()" @click="showLoginDialog" label="Login"/>
        <QBtn v-if="!authStore.isLoggedIn()" to="register" label="Register"/>

        <!-- section for logged in users -->
          <UserIcon v-if="authStore.isLoggedIn()" :username="authStore.user.username"/>
        </div>
        <QBtn v-if="authStore.isLoggedIn()" dense flat round icon="menu" @click="toggleRightDrawer"/>
      </QToolbar>
    </QHeader>

    <!-- section for logged in users -->
    <!-- TODO: add links to pages and functions -->
    <QDrawer v-if="authStore.isLoggedIn()" side="right" v-model="rightDrawerOpen" overlay bordered class="bg-grey-9">
      <QScrollArea class="fit">
      <QList>
        <QItem clickable>
          <QItemSection avatar>
            <QAvatar size="30px">
    <!-- TODO: add avatar icon -->
    <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
  </QAvatar>
          </QItemSection>
          <QItemSection>
            {{ authStore.user.username }}
          </QItemSection>
        </QItem>
        <QItem clickable>
          <QItemSection avatar>
            <QAvatar size="30px">

              <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
            </QAvatar>
          </QItemSection>
          <QItemSection>
            Profile
          </QItemSection>
        </QItem>
        <QItem clickable href="/createproject">
          <QItemSection avatar>
            <QAvatar size="30px">

              <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
            </QAvatar>
          </QItemSection>
          <QItemSection>
            Create project
          </QItemSection>
        </QItem>
        <QItem clickable>
          <QItemSection avatar>
            <QAvatar size="30px">

              <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
            </QAvatar>
          </QItemSection>
          <QItemSection>
            User settings
          </QItemSection>
        </QItem>
        <QItem clickable>
          <QItemSection avatar>
            <QAvatar size="30px">

              <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
            </QAvatar>
          </QItemSection>
          <QItemSection>
            User list
          </QItemSection>
        </QItem>
        <QItem clickable @click="logout">
          <QItemSection avatar>
            <QAvatar size="30px">

              <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
            </QAvatar>
          </QItemSection>
          <QItemSection>
            Logout
          </QItemSection>
        </QItem>
        </QList>
        </QScrollArea>
    </QDrawer>

    <QPageContainer>
      <router-view />
    </QPageContainer>

    <!--App footer-->
    <QFooter reveal bordered class="bg-grey-8 text-white">
      <QBar>
        <QSpace/>
        <!-- links to credits page -->
        <div>
          <RouterLink to="sandbox">Sandbox</RouterLink>
          <RouterLink to="credits">Credits</RouterLink>
        </div>
      </QBar>
    </QFooter>

  </QLayout>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { Dialog, Loading } from 'quasar';

import UserIcon from '@/components/UserIcon.vue';
import LoginDialog from '@/dialogs/LoginDialog.vue';
import router from '@/router/router';

import { useAuthStore } from '@/stores/AuthStore';

//Store for getting logged in user info
const authStore = useAuthStore();

//If true, shows a list of user quick links
const rightDrawerOpen = ref(false);

//Opens or closes drawer for user to go to other pages
const toggleRightDrawer = () => {
  rightDrawerOpen.value = !rightDrawerOpen.value;
}

function logout(){
    //Shows loading screen
    Loading.show({
      message: "Logging out..."
    });
  authStore.logout();
  Loading.hide();
}

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

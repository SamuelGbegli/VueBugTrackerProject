
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
        <div class="q-pa-md row items-center q-gutter-md">
          <QBtn to="/" label="home"/>
          <QBtn to="/browse" label="Browse"/>
          <QBtn to="/search" label="Search"/>
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
        <QItem v-if="authStore.getUserRole() != AccountRole.Normal" clickable href="/userlist">
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
      <router-view :key="route.fullPath" />
    </QPageContainer>

    <!--App footer-->
    <QFooter reveal bordered class="bg-grey-8 text-white">
      <QBar>
        <QSpace/>
        <!-- links to credits page -->
        <div>
          <RouterLink to="/sandbox">Sandbox</RouterLink>
          <RouterLink to="/credits">Credits</RouterLink>
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
import { useRoute } from 'vue-router';
import router from '@/router/router';

import { useAuthStore } from '@/stores/AuthStore';
import AccountRole from '@/enumConsts/Role';

//Store for getting logged in user info
const authStore = useAuthStore();

//Gets the current page's route
const route = useRoute();

//If true, shows a list of user quick links
const rightDrawerOpen = ref(false);

//Opens or closes drawer for user to go to other pages
const toggleRightDrawer = () => {
  rightDrawerOpen.value = !rightDrawerOpen.value;
}

async function logout(){
    //Shows loading screen
    Loading.show({
      message: "Logging out..."
    });
  await authStore.logout();
  Loading.hide();
  router.go(0);
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

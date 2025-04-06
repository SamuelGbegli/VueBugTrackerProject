<!--Dummy page to test components-->
<template>
  <h2>Sandbox</h2>
  <p>This page is for testing components only and is not part of the main application.</p>
  <LoginPrompt v-if="!isLoggedIn"/>
  <p v-else>User is logged in.</p>

  <ProjectForm/>
</template>
<script setup lang="ts">
  import LoginPrompt from '@/components/LoginPrompt.vue';
import ProjectForm from '@/components/ProjectForm.vue';
import Visibility from '@/enumConsts/Visibility';
  import { useAuthStore } from '@/stores/AuthStore';
import { onMounted, ref } from 'vue';

  const authStore = useAuthStore();
  const isLoggedIn = ref();

  onMounted(async ()=> {
    isLoggedIn.value = await authStore.isLoggedInBackend();
  });

</script>

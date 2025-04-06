<template>
  <div v-if="isLoggedIn">
    <h3>Create project</h3>
    <br/>
    <QBanner>
      Use the form below to create a new project.
    </QBanner>
    <ProjectForm/>
  </div>
  <div v-else>
    <LoginPrompt/>
  </div>
</template>
<script setup lang="ts">
  import { useAuthStore } from '@/stores/AuthStore';
  import LoginPrompt from '@/components/LoginPrompt.vue';
import { onMounted, ref } from 'vue';
import ProjectForm from '@/components/ProjectForm.vue';

const authStore = useAuthStore();
const isLoggedIn = ref();

  onMounted(async() =>{
    isLoggedIn.value = await authStore.isLoggedInBackend();
  })

</script>

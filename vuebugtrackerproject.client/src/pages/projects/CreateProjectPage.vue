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
    <ErrorBanner :promptType="ErrorPromptType.LoginAndGoBackButtons"/>
  </div>
</template>
<script setup lang="ts">
  import { useAuthStore } from '@/stores/AuthStore';
  import ErrorBanner from '@/components/ErrorBanner.vue';
import { onMounted, ref } from 'vue';
import ProjectForm from '@/components/ProjectForm.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';

const authStore = useAuthStore();
const isLoggedIn = ref();

  onMounted(async() =>{
    isLoggedIn.value = await authStore.isLoggedInBackend();
  })

</script>

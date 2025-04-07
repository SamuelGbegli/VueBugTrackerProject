<!--Dummy page to test components-->
<template>
  <h2>Sandbox</h2>
  <p>This page is for testing components only and is not part of the main application.</p>

  <ProjectPreview :-project="project"/>
</template>
<script setup lang="ts">
import ProjectPreview from '@/components/ProjectPreview.vue';
  import { useAuthStore } from '@/stores/AuthStore';
import AccountViewModel from '@/viewmodels/AccountViewModel';
import ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import { onMounted, ref } from 'vue';

  const authStore = useAuthStore();
  const isLoggedIn = ref();
  let project = ref(new ProjectPreviewViewModel());

  onMounted(async ()=> {
    let ppvm = new ProjectPreviewViewModel();
    ppvm.name = "Test project";
    ppvm.summary = "Summary";
    ppvm.totalBugs = 3
    ppvm.dateModified = new Date();
    ppvm.ownerName = "Test user"
    ppvm.ownerIcon = "https://archives.bulbagarden.net/media/upload/c/c2/Menu_HOME_0940.png";
    project.value = ppvm;

    console.log(project.value);
  });

</script>

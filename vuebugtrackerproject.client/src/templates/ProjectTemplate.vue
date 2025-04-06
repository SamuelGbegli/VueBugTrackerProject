<template>
  <QBanner>
    <h3>{{ project }}</h3>
    <h6>Project summary goes here</h6>
    <h6>Last updated timestamp goes here</h6>
  </QBanner>
  <router-view />
</template>
<script setup lang="ts">
import axios from 'axios';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '@/router/router';

const project = ref();
const route = useRoute();

onMounted(async ()=>{
  const response = await axios.get(`/projects/get/${route.params.projectId}`);

  project.value = response.data;
})
</script>

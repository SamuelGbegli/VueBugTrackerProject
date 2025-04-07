<template>
  <QBanner>
    <h3>{{ project.name }}</h3>
    <h6>{{ project.summary }}</h6>
    <h6>Last updated: {{ project.dateModified }}</h6>
  </QBanner>
  <QTabs align="left">
    <QRouteTab label="Main" :to="`/project/${project.id}`"/>
    <QRouteTab label="Bugs" :to="`#`"/>
    <!--TODO: hide if user does not own project-->
    <QRouteTab label="Settings" :to="`#`"/>
  </QTabs>
  <router-view />
</template>
<script setup lang="ts">
import axios from 'axios';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '@/router/router';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';

const project = ref(new ProjectViewModel());
const route = useRoute();

onMounted(async ()=>{

  //TODO: add error handling if project cannot be fetched from database
  const response = await axios.get(`/projects/get/${route.params.projectId}`);

  project.value = Object.assign(new ProjectViewModel(), response.data);
  console.log(project.value);
})
</script>

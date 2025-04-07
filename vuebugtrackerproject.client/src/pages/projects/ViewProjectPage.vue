<template>
<div class="q-pa-md row q-gutter-md">
  <div class="col-8">
    <QCard style="min-height: 60vh">
      <QCardSection>
        <h5>Description</h5>
        <div v-html="(project.description != '' ? project.description : 'No description provided')"/>
      </QCardSection>
    </QCard>
  </div>
  <div class="col">
    <QCard style="min-height: 60vh;">
      <QCardSection>
        <h5>Project details</h5>
      </QCardSection>
      <QCardSection>
        <h6>Created on {{ project.dateCreated }}</h6>
      </QCardSection>
      <QCardSection>
        <h6>Owner</h6>
        <UserIcon :username="project.ownerName" :icon="project.ownerIcon"/>
      </QCardSection>
      <QCardSection>
        <h6>Project link</h6>
        <a v-if="project.link != ''" :href="project.link"/>
        <span v-else>None provided</span>
      </QCardSection>
      <QCardSection>
        <h6>Number of bugs</h6>
        <span>{{ project.totalBugs }} ({{ project.openBugs }} open)</span>
      </QCardSection>
      <QCardSection>
        <h6>Tags</h6>
        <div>
          <QChip color="blue" v-for="x in project.tags">
            {{ x }}
          </QChip>
        </div>
      </QCardSection>
    </QCard>
  </div>
</div>
</template>
<script setup lang="ts">

import axios, { AxiosError } from 'axios';
import { onBeforeMount, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '@/router/router';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import UserIcon from '@/components/UserIcon.vue';

  const project = ref(new ProjectViewModel());
  const statusCode = ref();
  const route = useRoute();

  onBeforeMount(async ()=>{
    //TODO: add error handling for when loading project fails
    try{
      const response = await axios.get(`/projects/get/${route.params.projectId}`);
      statusCode.value = response.status;
      project.value = response.data;
    }
    catch (ex){
      let error = ex as AxiosError;
      statusCode.value = error.status;
    }
  });
</script>

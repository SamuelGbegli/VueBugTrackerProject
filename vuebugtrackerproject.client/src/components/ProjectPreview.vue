<!--Shows a preview of a project when browsing-->
<template>
  <QCard style="width:100%; max-width: 250px;">
    <QCardSection>
      <div class="text-h5">{{ Project?.name }}</div>
      <div class="text-subtitle2">{{ Project?.summary }}</div>
      <UserIcon
      :username="Project?.ownerName"
      :icon="Project?.ownerIcon"/>
     <div class="text-subtitle2">Total bugs: {{ Project?.totalBugs }} ({{ Project?.openBugs }} open)</div>
      <div class="text-subtitle2">Last updated: {{ formatDate(Project.dateModified) }}</div>
    </QCardSection>
    <QCardActions align="right">
      <QBtn :to="`/project/${Project?.id}`" label="View"/>
    </QCardActions>
  </QCard>
</template>
<script setup lang="ts">

import ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import { onMounted, ref } from 'vue';
import UserIcon from './UserIcon.vue';
import formatDate from '@/classes/helpers/FormatDate';

const props = defineProps({
  Project: ProjectPreviewViewModel
});

</script>

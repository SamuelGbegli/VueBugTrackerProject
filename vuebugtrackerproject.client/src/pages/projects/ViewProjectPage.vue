<template>

</template>
<script setup lang="ts">

import axios from 'axios';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '@/router/router';

  const project = ref();
  const route = useRoute();

  onMounted(async ()=>{
    const response = await axios.get(`/projects/get/${route.params.projectId}`)

    console.log(route.params.projectId);
    console.log(response);
  });
</script>

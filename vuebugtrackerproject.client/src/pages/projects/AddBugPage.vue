<template>
  <div v-if="!statusCode || statusCode === 200">
    <h4>Add bug</h4>
    <QBanner>
      Use the form below to add a new bug.
    </QBanner>
      <br/>
    <BugForm/>
    <QInnerLoading
    :showing="statusCode === 0"/>
  </div>
  <ErrorBanner v-else
    :prompt-type=" ErrorPromptType.GoBackButtonOnly"
    message="The page you requested does not exist."/>
</template>
<script setup lang="ts">
  import BugForm from '@/components/BugForm.vue';
import axios from 'axios';
import { onBeforeMount, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/AuthStore';
import ErrorBanner from '@/components/ErrorBanner.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';

const statusCode = ref(0);
const route = useRoute();
const authStore = useAuthStore();

//Used to verify if the user has created the project
  onBeforeMount(async () =>{
    try{
      const response = await axios.get(`/projects/get/${route.params.projectId}`);
      if(response.status === 200){
        if(response.data.ownerID === authStore.getUserID()) statusCode.value = 200;
        else statusCode.value = 403;
      }
      else statusCode.value = response.status;
    }
    catch(ex){

    }
  });

</script>

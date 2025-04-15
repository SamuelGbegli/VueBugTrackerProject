<template>
  <div class="q-pa-md row q-gutter-md">
    <div class="col-8">
      <QCard style="min-height: 60vh">
        <QCardSection v-if="statusCode === 200">
          <h5>Description</h5>
          <span>{{ bug.description }}</span>
        </QCardSection>
        <QCardSection v-else>
          <QSkeleton square height="500px"/>
        </QCardSection>
      </QCard>
    </div>
    <div class="col">
      <QCard>
        <div v-if="statusCode === 200">
          <QCardSection>
          <h5>Bug details</h5>
        </QCardSection>
        <QCardSection>
          <h6>Created on {{ formatDate(bug.dateCreated) }}</h6>
        </QCardSection>
        <QCardSection>
          <h6>Created by</h6>
          <UserIcon :username="bug.creatorName" :icon="bug.creatorIcon"/>
        </QCardSection>
        <QCardSection>
          <h6>Status</h6>
          <span>{{ bug.isOpen ? "Open" : "Closed" }}</span>
        </QCardSection>
        <QCardSection>
          <h6>Severity</h6>
          <span>{{ bugSeverity[bug.severity] }}</span>
        </QCardSection>
        <QCardSection>
          <QBtn @click="showBugToggleDialog" :label="bug.isOpen ? 'Close bug' : 'Reopen bug'"/>
        </QCardSection>
        </div>
        <div v-else>
          <QCardSection>
            <QSkeleton square height="500px"/>
          </QCardSection>
        </div>
      </QCard>
    </div>
  </div>
  <QDialog v-model="showDialog" persistent>
    <QCard>
      <QCardSection>
        <h5>
          {{ bug.isOpen ? "Close bug"
        : "Reopen bug" }}
        </h5>
      </QCardSection>
      <QCardSection>
        {{ bug.isOpen ? "Are you sure you want to close this bug?"
        : "Are you sure you want to reopen this bug?" }}
      </QCardSection>
      <QCardActions>
        <QBtn label="Yes" @click="toggleBug" v-close-popup/>
        <QBtn label="No" v-close-popup/>
      </QCardActions>
    </QCard>
  </QDialog>
  </template>
  <script setup lang="ts">

  import axios, { AxiosError } from 'axios';
  import { onBeforeMount, onMounted, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import UserIcon from '@/components/UserIcon.vue';
import BugViewModel from '@/viewmodels/BugViewModel';
import { Dialog, Loading, Notify } from 'quasar';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import formatDate from '@/classes/helpers/FormatDate';

const bugSeverity = ["Low", "Medium", "High"];

    const bug = ref(new BugViewModel());
    const statusCode = ref();
    const route = useRoute();
    const router = useRouter();
    const showDialog = ref(false);

    onBeforeMount(async ()=>{
      //TODO: add error handling for when loading project fails
      try{
        const response = await axios.get(`/bugs/get/${route.params.bugId}`);
        statusCode.value = response.status;
        bug.value = response.data;
      }
      catch (ex){
        let error = ex as AxiosError;
        statusCode.value = error.status;
      }
    });

  function showBugToggleDialog(){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        header: bug.value.isOpen ? "Close bug" : "Reopen bug",
        message: bug.value.isOpen ? "Are you sure you want to close this bug?"
        : "Are you sure you want to reopen this bug?"
      }
    }).onOk(async () =>{
      await toggleBug();
    })
  }

//Opens a closed bug, and vice versa
    async function toggleBug(){
      Loading.show({
        message: "Please wait..."
      });
      try{
        const response = await axios.patch("/bugs/togglebugstatus", bug.value.id,{
          headers: {"Content-Type": "application/json"}
        });
        router.go(0);
      }
      catch{
        Notify.create({
          message: "Something went wrong when processing your request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      Loading.hide();
    }
  </script>

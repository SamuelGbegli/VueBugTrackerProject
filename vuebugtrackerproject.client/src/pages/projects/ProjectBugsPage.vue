<template>
  <div class="q-pa-md">
  <div class="row">
    <div class="col">
      <h6>Number of bugs: {{ totalBugs }}</h6>
    </div>
    <div class="col-2">
      <QBtn :to="`/project/${route.params.projectId}/bugs/add`" label="Add bug"/>
      <QBtn label="Filter"/>
    </div>
  </div>
  <div v-if="!!bugs && bugs.length > 0">
    <QMarkupTable>
        <thead>
          <tr>
            <th>Description</th>
            <th>Last updated</th>
            <th>Created by</th>
            <th>Severity</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="x in bugs">
            <td>{{ (x as BugPreviewViewModel).summary }}</td>
            <td>{{ formatDate((x as BugPreviewViewModel).dateModified) }}</td>
            <td>
              <UserIcon :username="(x as BugPreviewViewModel).creatorName"/>
            </td>
            <td>
              <QChip :color="getChipColour(x.severity)">
                {{ (x as BugPreviewViewModel).severity }}
              </QChip>
            </td>
            <td>
              <QChip :color="getChipColour(x.status)">
                {{ (x as BugPreviewViewModel).status }}
              </QChip>
            </td>
            <td>
              <QBtnDropdown v-if="(x as BugPreviewViewModel).creatorID === authStore.getUserID() || project.ownerID === authStore.getUserID()"
                label="View"
                :to="`/bug/${x.id}`">
                <QList>
                  <QItem clickable v-close-popup>
                    <QItemSection>
                      <QItemLabel>Edit</QItemLabel>
                    </QItemSection>
                  </QItem>
                  <QItem clickable v-close-popup>
                    <QItemSection>
                      <QItemLabel>Delete</QItemLabel>
                    </QItemSection>
                  </QItem>
                </QList>
              </QBtnDropdown>
              <QBtn v-else label="View" :to="`/bug/${x.id}`"/>
            </td>
          </tr>
        </tbody>
      </QMarkupTable>
      <QPagination v-model="currentPage"
        :min="1"
        :max="numberOfPages"
        input
        @update:model-value="getBugs"/>
      <QInnerLoading
      :showing="loading"
      label="Please wait..."/>
  </div>
  <h5 v-else>This project has no bugs.</h5>
  </div>
</template>
<script setup lang="ts">
import UserIcon from '@/components/UserIcon.vue';
import BugPreviewViewModel from '@/viewmodels/BugPreviewViewModel';
import axios from 'axios';
import { onBeforeMount, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/AuthStore';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import formatDate from '@/classes/helpers/FormatDate';

//The number of pages of project bugs in the backend, in groups of 20
const numberOfPages = ref(5);

//The current page the user is on
const currentPage = ref(1);

//The number of bugs the project has
const totalBugs = ref(0);

//The bugs returned by the server
const bugs = ref();

//Stores project information
const project = ref(new ProjectViewModel());

//If true, shows a loading animation on the bug list
const loading = ref(false);

const route = useRoute();
const authStore = useAuthStore();

onBeforeMount(async() =>{
  await getBugs();
  const response = await axios.get(`/bugs/getnumberofbugs/${route.params.projectId}`);
  totalBugs.value = response.data;
  numberOfPages.value = Math.ceil(totalBugs.value/20);

  const projectResponse = await axios.get(`/projects/get/${route.params.projectId}`);
  project.value = projectResponse.data;

  console.log(project.value);
});

//Gets bugs from the server
async function getBugs(){
  loading.value = true;
  try{
    const response = await axios.get(`/bugs/getbugpreviews?projectId=${route.params.projectId}&page=${currentPage.value}`)
    let data: BugPreviewViewModel[] = [];
    for(let i = 0; i < response.data.length; i ++){
      data.push(Object.assign(new BugPreviewViewModel(), response.data[i]));
    }
    bugs.value = data;
  }
  catch(ex){

  }
  finally{
    loading.value = false;
  }
}

//Assigns chip colour to bug severity and status fields
function getChipColour(value: string){

  switch(value){
    case "Low":
      return "blue";
    case "Medium":
      return "amber";
    case "High":
    case "Closed":
      return "red";
    case "Open":
      return "green";
  }
}

</script>

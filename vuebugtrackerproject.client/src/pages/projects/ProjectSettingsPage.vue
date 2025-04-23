<template>
  <br/>
  <div>
    <div class="q pa-md q-gutter-md row">
      <div class="col-1">
        <QTabs
          v-model="currentTab"
          vertical>
          <QTab name="Edit project" label="Edit project"/>
          <QTab name="Permissions" label="Permissions"/>
          <QTab name="Other" label="Other"/>
        </QTabs>
      </div>
      <div class="col">
        <QTabPanels v-model="currentTab"
         vertical>
         <!--Section to edit a project-->
          <QTabPanel name="Edit project">
            <div>
              <QBanner>
                Use the form below to edit the project.
              </QBanner>
              <br/>
              <div>
              <ProjectForm :-project="project"/>
              </div>
            </div>
          </QTabPanel>
          <!--Section to edit user permissions-->
          <QTabPanel name="Permissions">
            <div class="row">
              <!--TODO: add function to add user permission-->
              <QSpace/>
              <QBtn label="Add user"/>
            </div>
            <br/>
            <div>
              <QMarkupTable>
              <thead>
                <tr>
                  <th>User</th>
                  <th>Role</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <!--TODO: add rows for user permisiond-->
              </tbody>
            </QMarkupTable>
            <div class="row">
              <QSpace/>
              <!--TODO: add pagination control-->
            </div>
            </div>
          </QTabPanel>
          <!--Section to delete project-->
          <QTabPanel name="Other">
            <QCardSection>
              <div class="row">
                <div class="col">
                  Click the button on the right to delete the project. This is an irreversable process, and you will be asked to confirm your choice.
                </div>
                <div class="col-2">
                  <QBtn @click="deleteProject" label="Delete project"/>
                </div>
              </div>
            </QCardSection>
          </QTabPanel>
         </QTabPanels>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import ProjectForm from '@/components/ProjectForm.vue';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import axios, { AxiosError } from 'axios';
import { Dialog, Loading, Notify } from 'quasar';
import { onBeforeMount, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';


  const project = ref(new ProjectViewModel());
  const statusCode = ref();
  const route = useRoute();
  const router = useRouter();
  const currentTab = ref("Edit project");

  onBeforeMount(async () =>{
    //TODO: add error handling for when loading project fails
    try{
        const response = await axios.get(`/projects/get/${route.params.projectId}`);
        statusCode.value = response.status;
        project.value = Object.assign(new ProjectViewModel(), response.data);
        console.log(project.value);
      }
      catch (ex){
        let error = ex as AxiosError;
        statusCode.value = error.status;
      }
  });

  function deleteProject(){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        header: "Delete project",
        message: "Are you sure you want to delete this project? This is an irreversable process and all bugs and comments created will also be deleted.",
        requiresConfirmation: true

      }
    }).onOk(async () =>{
      Loading.show({
        message: "Please wait..."
      });

      try{
        const response = await axios.delete("/projects/delete", {
        headers:{
          "Content-Type": "application/json"
        },
        data: route.params.projectId
      });
      //Redirects to user page
      router.push(`/browse`);
      }
      catch{
        Notify.create({
          message: "Could not process request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      finally{
        Loading.hide;
      }
    });
    }

</script>

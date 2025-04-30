<template>
  <br/>
  <div>
    <div class="q pa-md q-gutter-md row">
      <div class="col-2">
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
              <QSpace/>
              <QBtn @click="openDialog()" label="Add user"/>
            </div>
            <br/>
            <div v-if="userPermissions.totalPermissions > 0">
              <QMarkupTable>
              <thead>
                <tr>
                  <th>User</th>
                  <th>Role</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="x in userPermissions.userPermissions">
                  <td>
                    <UserIcon :username="(x as UserPermissionViewModel).accountName" />
                  </td>
                  <td>
                    {{
                      (x as UserPermissionViewModel).permission === ProjectPermission.Owner?
                      "Owner" :
                      (x as UserPermissionViewModel).permission === ProjectPermission.Editor?
                      "Editor" :
                      "Viewer"
                     }}
                  </td>
                  <td>
                  <!--TODO: Add actions for non-project owners-->
                  <div v-if="project.ownerID != (x as UserPermissionViewModel).accountID">
                    <QBtnDropdown v-if="project.visibility === Visibility.Restricted" label="Actions">
                    <QList>
                      <QItem clickable v-close-popup @click="openDialog(x)">
                        <QItemSection>
                          <QItemLabel>Edit</QItemLabel>
                        </QItemSection>
                      </QItem>
                      <QItem clickable v-close-popup @click="deleteUserPermission(x)">
                        <QItemSection>
                          <QItemLabel>Delete</QItemLabel>
                        </QItemSection>
                      </QItem>
                    </QList>
                    </QBtnDropdown>
                    <QBtn v-else @click="deleteUserPermission(x as UserPermissionViewModel)" label="Delete"/>
                  </div>
                  </td>
                </tr>
              </tbody>
            </QMarkupTable>
            <div class="row">
              <QSpace/>
              <QPagination :min="1" :max="userPermissions.pages"
              v-model="pageNumber"
              @update:model-value="loadUserPermissions"
              input/>
            </div>
            </div>
            <h5 v-else>There are no user permissions.</h5>
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
  <QDialog v-model="showDialog">
    <QCard style="min-width: 400px;">
      <QCardSection>
        <div class="row">
          <h6>
            {{ !!selectedPermission ? "Edit permission" : "Add permission" }}
          </h6>
          <QSpace/>
          <QBtn icon="close" flat round dense v-close-popup/>
        </div>
      </QCardSection>
      <QForm @submit="submitForm">
        <QCardSection>
          <!--Username field only visible if adding user-->
          <QInput label="Username"
          v-if="!selectedPermission"
          stack-label
          v-model="permissionFormValues.username"
          :rules="[val => validateUsername(val)]"
          />
          <!--If editing, shows name of user whose permission is being edited-->
          <h6 v-if="!!selectedPermission">Editing {{ selectedPermission.accountName }}</h6>
          <!--Role dropdown only visible if project is restricted-->
          <QSelect v-if="(project.visibility === Visibility.Restricted)"
          v-model="permissionFormValues.role"
          label="Role"
          stack-label
          :options="permissionRoles"/>
        </QCardSection>
        <QCardActions align="right">
          <QBtn type="submit" label="submit"/>
        </QCardActions>
      </QForm>
    </QCard>
  </QDialog>
</template>
<script setup lang="ts">
import UserPermissionDTO from '@/classes/DTOs/UserPermissionDTO';
import ProjectForm from '@/components/ProjectForm.vue';
import UserIcon from '@/components/UserIcon.vue';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import ProjectPermission from '@/enumConsts/ProjectPermission';
import Visibility from '@/enumConsts/Visibility';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import UserPermissionContainer from '@/viewmodels/UserPermissionContainer';
import type UserPermissionViewModel from '@/viewmodels/UserPermissionViewModel';
import axios, { AxiosError } from 'axios';
import { Dialog, Loading, Notify } from 'quasar';
import { onBeforeMount, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

  const project = ref(new ProjectViewModel());
  const statusCode = ref();
  const route = useRoute();
  const router = useRouter();
  const currentTab = ref("Edit project");

  //Stores user permissions
  const userPermissions = ref(new UserPermissionContainer());
  //Stores user permission table page number
  const pageNumber = ref(1);
  //If true, shows dialog to add or edit a user permission
  const showDialog = ref(false);
  //The user permission that is being edited, if any
  const selectedPermission = ref();
  //The roles that can be assigned to a user
  const permissionRoles = ["Viewer", "Editor"];

  //Stores values in the permission form
  const permissionFormValues = ref({
    username: "",
    role: permissionRoles[ProjectPermission.Viewer]
  });

  onBeforeMount(async () =>{
    //TODO: add error handling for when loading project fails
    //Gets project from backend
    try{
        const response = await axios.get(`/projects/get/${route.params.projectId}`);
        statusCode.value = response.status;
        project.value = Object.assign(new ProjectViewModel(), response.data);
      }
      catch (ex){
        let error = ex as AxiosError;
        statusCode.value = error.status;
      }

      //Gets user permissions
      loadUserPermissions();
  });

  //Gets project user permissions from the backend
  async function loadUserPermissions() {
    try{
      const response = await axios.get(`/userpermissions/get?projectID=${route.params.projectId}&pageNumber=${pageNumber.value}`)
      userPermissions.value = Object.assign(new UserPermissionContainer, response.data);
      console.log(JSON.stringify(userPermissions.value));
      console.log(`/userpermissions/get?projectID=${route.params.projectId}&pageNumber=${pageNumber.value}`);
    }
    catch (ex){
      //TODO: add error functioning data fails to load
    }
  }

  //Prepares and opens the user permissions dialog
  function openDialog(userPermission: UserPermissionViewModel | null = null){
    if(!!userPermission) {
      selectedPermission.value = userPermission;
      permissionFormValues.value = ({
        username: userPermission.accountName,
        role: permissionRoles[userPermission.permission]
      });
    }
    else {
      selectedPermission.value = null;
      permissionFormValues.value = ({
        username: "",
        role: permissionRoles[ProjectPermission.Viewer]
      });
    };
    showDialog.value = true;
  }

  //Used to validate the username inputted in the user permission form
  async function validateUsername(input: string){
    //Checks if input is empty
    if(!input) return "Please fill out this field."

    try{
      //Checks if an account exists with the name given
      var response = await axios.get(`/userpermissions/isnamevalid?username=${input}&projectId=${project.value.id}`);
      //If response is OK, user exists and can have a permission added
      return true;
    }
    catch (ex){
      //Shows error message if an error happens
      let error = ex as AxiosError;
      console.log(error);
      return error.response?.data;
    }

  }

  //Submits user permission
  async function submitForm() {
    try {
      let permissionDTO = new UserPermissionDTO();

      permissionDTO.projectID = project.value.id;
      //NB: Uses username instead of ID since all usernames must be unique
      permissionDTO.accountID = permissionFormValues.value.username;

      //Checks if project is restricted; if not, user is assigned the editor role
      if(project.value.visibility === Visibility.Restricted)
        permissionDTO.permission = permissionRoles.indexOf(permissionFormValues.value.role);
      else permissionDTO.permission = ProjectPermission.Editor;

      //Block for when a value is being edited
      if(!!selectedPermission.value){
        //Gets permission ID
        permissionDTO.permissionID = (selectedPermission.value as UserPermissionViewModel).id;
        //Sends request to server
        console.log(JSON.stringify(permissionDTO));
        await axios.patch("/userpermissions/update", permissionDTO);
      }
      else{
        //Sends request to server
        await axios.post("/userpermissions/add", permissionDTO);
      }

      //Closes dialog
      showDialog.value = false;

      //Reloads user permissions
      //TODO: add function to change page if adding permission
      await loadUserPermissions();

      //Show notification
      Notify.create({
        message: !!selectedPermission.value ? "Successfully modified permission." : "Successfully added permission.",
        position: "bottom",
        type: "positive"
      });
    } catch {
      Notify.create({
      message: "Something went wrong when processing your request. Please try again later.",
      position: "bottom",
      type: "negative"
  });
    }
  }

  //Removes a user permission
  function deleteUserPermission(userPermission: UserPermissionViewModel){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        header:"Remove user permission",
        message: `Are you sure you want to remove ${userPermission.accountName} from this project's permission list?`
      }
    }).onOk(async () => {
      Loading.show({
        message: "Please wait..."
      });

      try{
        await axios.delete("/userpermissions/delete", {
          headers:{
            "Content-Type": "application/json"
          },
          data: userPermission.id
        });
        //Reloads user permissions, going back to page 1
        pageNumber.value = 1;
        await loadUserPermissions();
        //Show notification
      Notify.create({
        message: "Successfully deleted permission.",
        position: "bottom",
        type: "positive"
      });
      }
      catch{
        //Error message if process fails
        Notify.create({
          message: "Could not process request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      Loading.hide();
    })
  }

  //Deletes the project
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

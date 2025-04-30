<!--Page for modifying a user's settings-->
<template>
  <div>
    <h5>Account settings</h5>
    <div class="q-pa-md q-gutter-md row">
      <div class="col-2">
        <QTabs
          v-model="currentTab"
          vertical/>
      </div>
      <div class="col">
        <QTabPanels
          v-model="currentTab"
          vertical>
            <QTabPanel name="Avatar">
              <!--TODO: add section for user to change avatar-->
            </QTabPanel>
            <QTabPanel name="Credentials">
              <!--TODO: add section for user to change email, username and password-->
            </QTabPanel>
            <QTabPanel name="Other">
              <!--TODO: add section for user to delete account-->
            </QTabPanel>
          </QTabPanels>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';

//Stores the tab the user is currently on
const currentTab = ref("Avatar");
</script>

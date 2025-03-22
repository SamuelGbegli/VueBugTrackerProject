<template>
  <QAvatar size="30px" style="margin: 5px;">
    <!-- TODO: add avatar icon -->
    <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg"/>
  </QAvatar>
  <!--TODO: add user name-->
  <span>{{ username ? username : "Null username" }}</span>
</template>
<script setup lang="ts">

import { ref } from 'vue';

const props = defineProps({
  username: String
})


</script>

<template>
  <div class="q-gutter-sm">
    <QAvatar size="30px" style="margin: 5px;">
      <img :src="props.icon ? props.icon : 'https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg'"/>
    </QAvatar>
    <span>{{ username ? username : "Null username" }}</span>
  </div>
</template>
<script setup lang="ts">

import { ref } from 'vue';

const props = defineProps({
  username: String,
  icon: String
})

console.log(props.icon);

</script>

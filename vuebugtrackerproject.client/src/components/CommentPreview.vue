<template>
  <h6>{{ comment.edited? `${formatDate(comment.datePosted)} (Edited)` : `${formatDate(comment.datePosted)}` }}</h6>
  <QCard>
    <QCardSection>
      <UserIcon/>
      <QCard v-if="!!props.comment?.replyCommentID">
        <QCardSection>
          <UserIcon :username="props.comment.replyCommentOwnerName"/>
          <p>{{ props.comment.replyCommentText }}</p>
        </QCardSection>
      </QCard>
      <p>{{ props.comment?.text }}</p>
    </QCardSection>
  </QCard>
</template>
<script setup lang="ts">
import CommentViewModel from '@/viewmodels/CommentViewModel';
import UserIcon from './UserIcon.vue';
import formatDate from '@/classes/helpers/FormatDate';

const props = defineProps({
  comment: CommentViewModel
});

</script>

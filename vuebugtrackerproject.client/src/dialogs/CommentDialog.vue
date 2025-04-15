<template>
  <QDialog ref="dialogRef"
  persistent
  @hide="onDialogHide">
  <QCard class="q-dialog-plugin">
    <QCardSection>
      <div class="row">
        <h5>Post comment</h5>
      <QSpace/>
      <QBtn icon="close" flat v-close-popup />
      </div>
    </QCardSection>
    <QForm @submit="onSubmit">
      <QCardSection v-if="!!props.replyComment">
        <QCard>
          <QCardSection>
            <div>Replying to {{ props.replyComment.ownerName }}</div>
            <QSeparator/>
            <div>
              {{ props.replyComment.text }}
            </div>
          </QCardSection>
        </QCard>
      </QCardSection>
        <QCardSection>
        <QInput v-model="commentText"
                type="textarea"
                :rules="[val => !!val || 'Please fill out this field.']" />
      </QCardSection>
      <QCardActions align="right">
        <QBtn type="submit" label="Submit" />
      </QCardActions>
    </QForm>
  </QCard>
  </QDialog>
</template>
<script setup lang="ts">
  import { onBeforeMount, ref } from 'vue';

  import { useDialogPluginComponent, Loading } from 'quasar';
  import UserDTO from '@/classes/DTOs/UserDTO';
import CommentViewModel from '@/viewmodels/CommentViewModel';
import { useAuthStore } from '@/stores/AuthStore';
import { useRoute } from 'vue-router';
import CommentDTO from '@/classes/DTOs/CommentDTO';

const authStore = useAuthStore();
const route = useRoute();


  const props = defineProps({
    //Comment to be replied to
    replyComment: CommentViewModel,
    //Existing comment, if being edited
    existingComment: CommentViewModel
  });

  //The text that will be submitted when posting a new comment
  const commentText = ref("");

  defineEmits([
    ...useDialogPluginComponent.emits
  ]);

  const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent();

onBeforeMount(() =>{
  //Adds comment text to form if it already exists
  if(!!props.existingComment)
    commentText.value = props.existingComment.text;
})

//Posts the comment
async function onSubmit() {
  let commentDTO = new CommentDTO();

    commentDTO.ownerID = authStore.getUserID();
    commentDTO.bugID = route.params.bugId.toString();
    commentDTO.text = commentText.value;

    //Adds comment ID if it's being edited
    if(!!props.existingComment)
      commentDTO.commentID = props.existingComment.id;

    //Adds ID of reply comment if it exists
    if(!!props.replyComment)
      commentDTO.replyID = props.replyComment.id;

    console.log(commentDTO);
    onDialogOK();
    //TODO: add post and push functions for comments
}
</script>

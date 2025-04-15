<template>
  <QCard>
    <QCardSection>
      Use the form below to add a comment.
    </QCardSection>
    <QForm @submit="postComment">
      <QCardSection>
        <QBanner v-if="!!replyComment">
          <CommentPreview :comment="replyComment" />
        </QBanner>
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
  <QSeparator />
  <div class="row">
    <h6>Total comments: {{ commentContainer.totalComments }}</h6>
    <QSpace/>
    <QBtn @click="showCommentDialog()" label="Add comment"/>
  </div>
  <div v-if="commentContainer.totalComments > 0">
    <div v-for="x in commentContainer.comments">
      <CommentPreview :comment="(x as CommentViewModel)" />
      <div class="row" v-if="!x.isStatusUpdate">
        <QBtn @click="showCommentDialog(null, x)" label="Reply" />
        <QBtn v-if="x.ownerID === authStore.getUserID()" @click="showCommentDialog(x)" label="Edit" />
        <QBtn v-if="x.ownerID === authStore.getUserID()" label="Delete" />
      </div>
    </div>
    <QPagination v-model="currentPage"
                   :min="1"
                   :max="numberOfPages"
                   @update:model-value="getComments"
                   input />
  </div>
</template>
<script setup lang="ts">
  import CommentPreview from '@/components/CommentPreview.vue';
  import CommentContainer from '@/viewmodels/CommentContainer';
  import CommentViewModel from '@/viewmodels/CommentViewModel';
  import axios from 'axios';
  import { ref, onBeforeMount } from 'vue';
  import { useRoute } from 'vue-router';
  import { useAuthStore } from '@/stores/AuthStore';
  import CommentDTO from '@/classes/DTOs/CommentDTO';
  import { Dialog, Loading, Notify } from 'quasar';
import CommentDialog from '@/dialogs/CommentDialog.vue';

  //The text that will be submitted when posting a new comment
  const commentText = ref("");

  //The page of comments the user is on
  const currentPage = ref(1);

  //The number of pages of comments the bug has
  const numberOfPages = ref(5);

  //Stores the number of comments, and the current page and comments visible
  const commentContainer = ref(new CommentContainer());

  //Stores a comment that will be replied to by the user
  const replyComment = ref();

  const route = useRoute();
  const authStore = useAuthStore();

  onBeforeMount(async () => {
    await getComments();
    if (!!commentContainer.value) {
      numberOfPages.value = Math.ceil(commentContainer.value.totalComments / 20);
    }
  });

  //Loads comments from frontend
  async function getComments() {
    try {
      const response = await axios.get(`/comments/get/${route.params.bugId}/${currentPage.value}`);
      commentContainer.value = Object.assign(new CommentContainer, response.data);
    } catch {
      //TODO: add error handling for when comment fetching fails
    }
  };

  //Loads the comment to be replied to when the user clicks on a reply button
  function setupReplyComment(comment: CommentViewModel) {
    replyComment.value = comment;
    //TODO: take user to the top of the comment page
  }

  //Adds a comment to the bug
  async function postComment() {

    Loading.show({
      message: "Please wait..."
    });

    let commentDTO = new CommentDTO();

    commentDTO.ownerID = authStore.getUserID();
    commentDTO.bugID = route.params.bugId.toString();
    commentDTO.text = commentText.value;
    //TODO: add section for comment replies
    if (!!replyComment.value)
      commentDTO.replyID = replyComment.value.id;

    console.log(commentDTO);

    try {
      const response = await axios.post("/comments/add", commentDTO);
      await getComments();
      currentPage.value = Math.ceil(commentContainer.value.totalComments / 20);
      commentText.value = "";
      replyComment.value = null;
    }
    catch {
      Notify.create({
        message: "Something went wrong when processing your request. Please try again later.",
        position: "bottom",
        type: "negative"
      });
    }

    Loading.hide();
  }

  //Shows the comment dialog
  function showCommentDialog(comment: CommentViewModel | null = null, reply: CommentViewModel | null = null){
    Dialog.create({
      component: CommentDialog,
      componentProps:{
        existingComment: comment,
        replyComment: reply
      }
    }).onOk(() => {
      console.log("Clicked on OK");
    });
  }

</script>

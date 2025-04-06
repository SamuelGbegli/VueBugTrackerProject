<!--Form to create or save changes to a project-->
<!--TODO: add edit mode for existing projects-->
<template>
  <QCard style=" max-width: 1200px;">
    <QForm @submit="onSubmit" style="margin: 5px">
      <div class="q-pa-md example-row-equal-width">
        <div class="row q-gutter-lg">
          <div class="col" style="max-width: 400px;">
            <QInput label="Project name"
                    stack-label
                    v-model="projectName"
                    :rules="[val => !! val || 'Please fill out this field.']" />
          </div>
          <div class="col" style="max-width: 400px;">
            <QInput label="Summary"
                    stack-label
                    v-model="summary"
                    :rules="[val => !! val || 'Please fill out this field.']" />
          </div>
        </div>
      <div class="row q-gutter-lg">
        <div class="col" style="max-width: 400px;">
          <QInput label="Link"
                  stack-label
                  v-model="link" />
        </div>
        <div class="col" style="max-width: 400px;">
          <QSelect v-model="visibility" label="Visibility" stack-label :options="options"/>
        </div>
      </div>
      <br/>
      <div class=" q-gutter-md" style="max-width: 70%;">
        <p>Description</p>
        <QEditor v-model="description" min-height="5rem"/>
      <!--  <p>Description preview</p>
       <QCard flat bordered>
        <QCardSection v-html="description"/>
      </QCard> -->

      <QInput label="Tags (use ',' to separate tags)"
      stack-label
      v-model="tags"
      type="textarea"/>

      </div>

    </div>
    <QCardActions>
      <QBtn type="submit" label="Submit"/>
    </QCardActions>
    </QForm>
  </QCard>
  <!--TODO: add tooltips to help the user-->
</template>
<script setup lang="ts">
  import ProjectDTO from '@/classes/DTOs/ProjectDTO';
import removeHTMLTags from '@/classes/helpers/RemoveHTMLTags';
import sanitiseHTML from '@/classes/helpers/SanitiseHTML';
import Visibility from '@/enumConsts/Visibility';
import { ref } from 'vue';
import { Loading, Notify } from 'quasar'
import axios from 'axios';
import { useRouter } from 'vue-router';

  const options = ["Visible to everyone", "Visible to logged in users only", "Visible to selected users only"];

  const projectName = ref("");
  const summary = ref("");
  const link = ref("");
  const visibility = ref(options[Visibility.Public]);
  const description = ref("");
  const tags = ref("");

  const router = useRouter();

// Submits form to server
  async function onSubmit() {
    //TODO: send project DTO to backend

    Loading.show({
      message: "Please wait..."
    });

    let projectDTO = new ProjectDTO();
    projectDTO.Name = projectName.value;
    projectDTO.Summary = summary.value;
    projectDTO.Visibility = options.indexOf(visibility.value);
    projectDTO.Link = link.value;
    projectDTO.Description = removeHTMLTags(description.value);
    projectDTO.FormattedDescription = description.value;
    projectDTO.Tags = tags.value.split(",");

    // console.log(sanitiseHTML(description.value));
    // console.log(removeHTMLTags(description.value));
    // console.log(projectDTO);

    try{
      const response = await axios.post("/projects/create", projectDTO);
      if(response.status === 201){
        console.log(response.headers.location);
        router.push({path:response.headers.location});
      }
    }
    catch(ex){
      Notify.create({
    message: "Something went wrong when processing your request. Please try again later.",
    position: "bottom",
    type: "negative"
  });
    }

    Loading.hide();
  }

</script>

<!--Form to create or save changes to a project-->
<template>
  <QCard style=" max-width: 1200px;">
    <QForm @submit="onSubmit">
      <QCardSection>
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
      </QCardSection>
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
import { onBeforeMount, ref } from 'vue';
import { Loading, Notify } from 'quasar'
import axios from 'axios';
import { useRoute, useRouter } from 'vue-router';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';

  const props = defineProps({
    Project: ProjectViewModel
  })

  const options = ["Visible to everyone", "Visible to logged in users only", "Visible to selected users only"];

  const projectName = ref("");
  const summary = ref("");
  const link = ref("");
  const visibility = ref(options[Visibility.Public]);
  const description = ref("");
  const tags = ref("");

  const route = useRoute();
  const router = useRouter();

  //Populates values if project is being edited
  onBeforeMount(() =>{
    if(!!props.Project){
      projectName.value = props.Project.name;
      summary.value = props.Project.summary;
      link.value = props.Project.link;
      visibility.value = options[props.Project.visibility]
      description.value = props.Project.description;
      tags.value = props.Project.tags.join(",");

    console.log((props.Project));
    }
  });

// Submits form to server
  async function onSubmit() {

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


    try{
      //Block for editing project
      if(!!props.Project){
        projectDTO.ProjectID = route.params.projectId.toString();
        await axios.patch("/projects/update", projectDTO);
        router.go(0);
      }
      //Block for adding new project
      else{
        const response = await axios.post("/projects/create", projectDTO);
        if(response.status === 201){
          console.log(response.headers.location);
          router.push({path:response.headers.location});
        }
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

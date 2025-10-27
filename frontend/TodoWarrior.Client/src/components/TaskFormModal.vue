<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { TaskItem, NewTask, UpdateTask } from "../types";
import { toDateOnlyInput, toDateTimeLocalInput, fromDateTimeLocalInput } from "../utils/date";
import RetroCheckbox from "./RetroCheckbox.vue";
import RetroDatePicker from "./RetroDatePicker.vue";

const props = defineProps<{ open: boolean; model?: TaskItem | null }>();
const emit = defineEmits<{
  (e: "close"): void,
  (e: "save", payload: UpdateTask | NewTask): void
}>();

const title = ref("");
const description = ref<string|undefined>("");
const dueDate = ref<string>(toDateOnlyInput());
const reminderAt = ref<string>("");
const isActive = ref(true);
const isDone = ref(false);

watch(() => props.model, (m) => {
  if (m){
    title.value = m.title;
    description.value = m.description ?? "";
    dueDate.value = toDateOnlyInput(m.dueDate);
    reminderAt.value = toDateTimeLocalInput(m.reminderAt ?? undefined);
    isActive.value = m.isActive;
    isDone.value = m.isDone;
  }else{
    title.value = "";
    description.value = "";
    dueDate.value = toDateOnlyInput();
    reminderAt.value = "";
    isActive.value = true;
    isDone.value = false;
  }
}, { immediate: true });

const heading = computed(() => props.model ? "Edit Task" : "New Task");

function onSave(){
  const payload = {
    title: title.value.trim(),
    description: description.value?.trim(),
    dueDate: toDateOnlyInput(dueDate.value),
    reminderAt: fromDateTimeLocalInput(reminderAt.value),
    isActive: isActive.value,
    isDone: isDone.value
  };
  emit("save", props.model ? { ...payload, guid: props.model.guid } : payload);
}
</script>

<template>
  <div v-if="open" class="modal-mask" @keydown.esc.stop.prevent="emit('close')">
    <div class="modal" role="dialog" aria-modal="true">
      <header>
        <h3>{{ heading }}</h3>
        <button @click="emit('close')">X</button>
      </header>
      <div class="body">
        <div class="form-row">
          <label>Title</label>
          <input class="input" v-model="title" placeholder="Write a heroic title_" />
        </div>
        <div class="form-row">
          <label>Description</label>
          <textarea class="input" rows="3" v-model="description" placeholder="Optional details_" />
        </div>
        <div class="form-row form-two">
          <div>
            <label>Due date</label>
            <RetroDatePicker placeholder="Set due date_" v-model="dueDate" :timePicker="false" />
          </div>
          <div>
            <label>Reminder at</label>
            <RetroDatePicker placeholder="Set reminder_" v-model="reminderAt" />
          </div>
        </div>
        <div class="form-row form-two">
          <RetroCheckbox v-model="isDone">Done</RetroCheckbox>
        </div>
      </div>
      <div class="footer">
        <button @click="emit('close')">Cancel</button>
        <button :disabled="!title.trim()" @click="onSave">Save</button>
      </div>
    </div>
  </div>
</template>
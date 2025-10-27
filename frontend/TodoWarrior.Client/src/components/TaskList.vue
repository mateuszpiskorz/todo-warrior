<script setup lang="ts">
import { onMounted, ref, computed, onBeforeUnmount } from "vue";
import { storeToRefs } from "pinia";
import { useTaskStore } from "../stores/tasks";
import type { TaskItem } from "../types";
import TaskFormModal from "./TaskFormModal.vue";
import ConfirmDeleteModal from "./ConfirmDeleteModal.vue";
import RetroCheckbox from "./RetroCheckbox.vue";

const store = useTaskStore();
const { sortedTasks, loading } = storeToRefs(store);

const showForm = ref(false);
const editModel = ref<TaskItem | null>(null);
const showDelete = ref(false);
const pendingDeleteGuid = ref<string | null>(null);
const showCompleted = ref(false);

function openNew(){ 
  removeKeyListener();
  editModel.value = null; 
  showForm.value = true; 
}

function openEdit(t: TaskItem){
  removeKeyListener();
  editModel.value = t; 
  showForm.value = true; 
}

function closeNewEdit(){ 
  showForm.value = false; 
  editModel.value = null;
  addKeyListener();
}

function askDelete(t: TaskItem){ 
  editModel.value = t; 
  pendingDeleteGuid.value = t.guid; 
  showDelete.value = true; 
}

function addKeyListener(){
  window.addEventListener('keydown', onKey);
}

function removeKeyListener(){
  window.removeEventListener('keydown', onKey);
}

async function handleSave(payload: Partial<TaskItem>){
  if ((payload as any).guid){
    await store.updateTask(payload as any);
  } else {
    await store.addTask(payload as any);
  }
  showForm.value = false;
}

async function confirmDelete(){
  if (pendingDeleteGuid.value){
    await store.deleteTask(pendingDeleteGuid.value);
  }
  showDelete.value = false; pendingDeleteGuid.value = null;
}

onMounted(() => {
  store.fetchAllTasks();
  window.addEventListener('keydown', onKey);
});

onBeforeUnmount(() => window.removeEventListener('keydown', onKey));

function onKey(e: KeyboardEvent){
  if (e.key.toLowerCase() === 'n'){ openNew(); }
}

const empty = computed(() => !loading.value && sortedTasks.value.length === 0);
</script>

<template>
  <div>
    <div style="display:flex;gap:10px;align-items:center;margin-bottom:10px">
      <button @click="openNew">+ New Task</button>
      <RetroCheckbox :modelValue="showCompleted" @change="store.toggleShowCompleted">
      Show Completed
      </RetroCheckbox>
      <small style="color:var(--muted)">// Active tasks ordered by Due Date ^</small>
    </div>

    <table class="table" v-if="!empty">
      <thead>
        <tr>
          <th>Title</th>
          <th>Due</th>
          <th>Status</th>
          <th>Reminder</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="t in sortedTasks" :key="t.guid">
          <td>
            <span class="status-dot" :class="{done:t.isDone, inactive:!t.isActive}"></span>
            <strong>{{ t.title }}</strong>
            <div v-if="t.description" style="color:var(--muted);font-size:18px">{{ t.description}}</div>
          </td>
          <td>{{ t.dueDate }}</td>
          <td>{{ t.isDone ? 'Done' : 'Open' }}</td>
          <td>{{ t.reminderAt ? new Date(t.reminderAt).toLocaleString() : '—' }}</td>
          <td style="text-align:right">
            <div style="display:flex;gap:8px;justify-content:flex-end">
              <button @click="store.toggleComplete(t)">{{t.isDone ? 'Undo' : 'Done'}}</button>
              <button @click="openEdit(t)">Edit</button>
              <button @click="askDelete(t)">Delete</button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-else style="opacity:.8;padding:30px;text-align:center;border:1px dashed var(--grid);border-radius:10px">
      No active tasks. Press <span class="kbd">N</span> or click <em>New Task</em> to begin.
    </div>

    <TaskFormModal :open="showForm" :model="editModel" @close="closeNewEdit" @save="handleSave" />
    <ConfirmDeleteModal :open="showDelete" :title="editModel?.title" @cancel="showDelete=false" @confirm="confirmDelete" />
  </div>
</template>
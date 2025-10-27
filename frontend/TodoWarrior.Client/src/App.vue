<script setup lang="ts">
import RetroShell from "./components/RetroShell.vue";
import TaskList from "./components/TaskList.vue";
import Toasts from "./components/Toast.vue";
</script>

<template>
  <RetroShell>
    <TaskList />
  </RetroShell>
  <Toasts />
</template>

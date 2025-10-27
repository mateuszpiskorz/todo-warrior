<script setup lang="ts">
const props = defineProps<{ open: boolean; title?: string }>();
const emit = defineEmits<{(e:"confirm" | "cancel"): void}>();
</script>

<template>
  <div v-if="props.open" class="modal-mask" @keydown.esc.stop.prevent="emit('cancel')">
    <div class="modal" role="dialog" aria-modal="true">
      <header>
        <h3>Confirm delete</h3>
        <button @click="emit('cancel')">X</button>
      </header>
      <div class="body">
        <p>Are you sure to delete <strong>{{ title || 'this task' }}</strong>? </p>
      </div>
      <div class="footer">
        <button @click="emit('cancel')">Cancel</button>
        <button @click="emit('confirm')">Delete</button>
      </div>
    </div>
  </div>
</template>
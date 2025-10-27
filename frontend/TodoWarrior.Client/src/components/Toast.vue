<script setup lang="ts">
    import { storeToRefs } from 'pinia';
    import { useTaskStore } from '../stores/tasks';
    import { ref } from 'vue';

    const taskStore = useTaskStore();
    const { toasts } = storeToRefs(taskStore);
    const removingIds = ref<Set<number>>(new Set());

    function close(id: number) {
        removingIds.value.add(id);
        
        setTimeout(() => {
            taskStore.dismissToast(id);
            removingIds.value.delete(id);
        }, 300);
    }
</script>
<template>
    <div class="toast-wrap">
        <div v-for="t in toasts" :key="t.id" class="toast" :class="[t.type, {removing: removingIds.has(t.id)}]">
            <div style="display:flex;gap:10px;align-items:center;">
                <span class="status-dot" :class="{done:t.type==='success'}"></span>
                <div style="font-size:20px">{{ t.message }}</div>
                <button style="margin-left:auto" @click="close(t.id)">close</button>
            </div>
        </div>
    </div>
</template>

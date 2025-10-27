<script setup lang="ts">
import { computed } from 'vue';
import VueDatePicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css';

const props = withDefaults(defineProps<{ 
  modelValue?: string;
  placeholder?: string;
  timePicker?: boolean;
}>(), {
  timePicker: true,
  placeholder: 'Pick date & time_'
});

const emit = defineEmits<{ 
  (e: 'update:modelValue', value: string | null): void 
}>();

const internalValue = computed({
  get: () => props.modelValue ? new Date(props.modelValue) : null,
  set: (val: Date | null) => {
    if (val) {
      emit('update:modelValue', val.toISOString());
    } else {
      emit('update:modelValue', null);
    }
  }
});

const displayFormat = computed(() => 
  props.timePicker === false ? 'yyyy-MM-dd' : 'yyyy-MM-dd HH:mm'
);
</script>

<template>
  <VueDatePicker
    v-model="internalValue"
    :dark="true"
    :enable-time-picker="timePicker"
    :time-picker-inline="timePicker"
    :minutes-increment="5"
    :format="displayFormat"
    auto-apply
    :teleport="true"
  >
    <template #dp-input="{ value }">
      <input 
        class="input" 
        :value="value" 
        readonly 
        :placeholder="placeholder || 'Pick date & time'"
        style="cursor: pointer;"
      />
    </template>
  </VueDatePicker>
</template>

<style>
.dp__theme_dark {
  --dp-background-color: var(--panel);
  --dp-text-color: var(--text);
  --dp-hover-color: var(--grid);
  --dp-hover-text-color: var(--text);
  --dp-border-color: var(--grid);
  --dp-primary-color: var(--accent);
  --dp-primary-text-color: var(--bg);
  --dp-secondary-color: var(--muted);
  --dp-border-color-hover: var(--accent);
}

.dp__input {
  background: var(--panel) !important;
  border: 1px solid var(--grid) !important;
  color: var(--text) !important;
}

.dp__main {
  font-family: inherit;
}

.dp__menu {
  font-family: inherit;
}
</style>
<script setup lang="ts">
var props = defineProps<{
    modelValue: boolean;
    id?: string | null;
}>();

var emits = defineEmits<{
    (e: "update:modelValue", value: boolean): void;
}>();

</script>
<template>
    <div class="retro-checkbox">
      <label class="checkbox-container">
        <input type="checkbox" :id="props.id" v-model="props.modelValue" @change="$emit('update:modelValue', props.modelValue)" />
        <span class="checkmark"></span>
        <slot></slot>
      </label>
    </div>
</template>
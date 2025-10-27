<script setup lang="ts">
</script>


<template>
    <div class="grid-bg"></div>
    <div class="shell">
        <div class="header">
            <h1>> TodoWarrior</h1>
            <span class="badge">your@todo:~$ schedule --retro fighter</span>
            <span class="badge">//press <span class="kbd">N</span> to add</span>
        </div>
        <slot></slot>
    </div>
</template>
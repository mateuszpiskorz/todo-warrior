import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { useTaskStore } from './stores/tasks'
import App from './App.vue'
import './style.css'

createApp(App)
  .use(createPinia())
  .mount('#app')

const store = useTaskStore();
store.bindHub?.();

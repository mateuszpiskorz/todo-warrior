import { defineStore } from "pinia";
import { toDateOnlyInput } from "../utils/date"
import type { NewTask, TaskItem, UpdateTask } from "../types";
import * as api from "../services/api";

export interface Toast {
    id: number;
    message: string;
    type?: 'success' | 'error' | 'info';
}

export const useTaskStore = defineStore("tasks", {
    state: () => ({
        tasks: [] as TaskItem[],
        loading: false,
        showCompleted: false,
        filterDueDate: null as string | null,
        toasts: [] as Toast[],
        _toastId: 1
    }),
    getters: {
        sortedTasks(state) : TaskItem[] {
            let filtered = [...state.tasks]
                .filter(t => t.isActive && (!t.isDone || state.showCompleted));
            
            if (state.filterDueDate) {
                filtered = filtered.filter(t => {
                    const taskDueDate = toDateOnlyInput(t.dueDate);
                    return taskDueDate === state.filterDueDate;
                });
            }
            
            return filtered.sort((a, b) => 
                new Date(toDateOnlyInput(a.dueDate)).getTime() - 
                new Date(toDateOnlyInput(b.dueDate)).getTime()
            );
        }
    }, 
    actions: {
        toast(message: string, type: Toast['type'] = 'info') {
            const toast: Toast = { id: this._toastId++, message, type };
            this.toasts.push(toast);
            setTimeout(() => {
                this.dismissToast(toast.id);
            }, 3000);
        },
        dismissToast(id: number) {
            this.toasts = this.toasts.filter(t => t.id !== id);
        },
        setDueDateFilter(date: string | null) {
            this.filterDueDate = date;
        },
        clearDueDateFilter() {
            this.filterDueDate = null;
        },
        toggleShowCompleted() {
            this.showCompleted = !this.showCompleted;
        },
        async bindHub() {
            const { buildConnection, start, wireHandlers, dispose } = await import("../services/hub");
            const connection = buildConnection();

            connection.onreconnecting(() => this.toast("Reconnecting to server...", "info"));
            connection.onreconnected(() => this.toast("Reconnected", "success"));
            connection.onclose(() => this.toast("Disconnected from server", "error"));

            await start(connection);
            wireHandlers(connection, {
                ReminderDue: (data) => {
                    this.toast(`Reminder: ${data.title} is due on ${data.dueDate}`, "info");
                }
            });
        },
        async fetchAllTasks() {
            this.loading = true;
            try {
                this.tasks = await api.fetchTasks();
            } catch (error) {
                this.toast("Failed to fetch tasks", "error");
            } finally {
                this.loading = false;
            }
        },
        async addTask(newTask: NewTask) {
            try {
                const task = await api.addTask(newTask);
                this.toast("Task added successfully", "success");
                this.tasks.push(task);
            } catch (error) {
                this.toast("Failed to add task", "error");
                throw error;
            }
        },
        async toggleComplete(updateTask: UpdateTask) {
            try {
                var done = updateTask.isDone;
                updateTask.isDone = !done;
                const task = await api.updateTask(updateTask);
                const index = this.tasks.findIndex(t => t.guid === task.guid);
                if (index !== -1) {
                    this.tasks[index] = task;
                    this.toast(`Task successfully marked as ${done ? 'undone' : 'done'}`, "success");
                }
            } catch (error) {
                this.toast("Failed to toggle task completion", "error");
                throw error;
            }
        },
        async updateTask(updateTask: UpdateTask) {
            try {
                const task = await api.updateTask(updateTask);
                const index = this.tasks.findIndex(t => t.guid === task.guid);
                if (index !== -1) {
                    this.tasks[index] = task;
                    this.toast("Task updated successfully", "success");
                } 
            } catch (error) {
                this.toast("Failed to update task", "error");
                throw error;
            }
        },
        async deleteTask(guid: string) {
            try {
                await api.deleteTask(guid);
                this.tasks = this.tasks.filter(t => t.guid !== guid);
                this.toast("Task deleted successfully", "success");
            } catch (error) {
                this.toast("Failed to delete task", "error");
                throw error;
            }
        },
    },
});
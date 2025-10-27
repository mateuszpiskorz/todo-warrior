import axios from 'axios';
import type { TaskItem, UpdateTask, NewTask } from '../types';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/';
const http = axios.create({baseURL: BASE_URL});

export async function fetchTasks(): Promise<TaskItem[]> {
    const response = await http.get<TaskItem[]>('api/tasks');
    return response.data;
}

export async function addTask(newTask: NewTask): Promise<TaskItem> {
    const response = await http.post<TaskItem>('api/tasks', newTask);
    return response.data;
}

export async function updateTask(updateTask: UpdateTask): Promise<TaskItem> {
    const response = await http.put<TaskItem>(`api/tasks/${updateTask.guid}`, updateTask);
    return response.data;
}

export async function deleteTask(guid: string): Promise<void> {
    await http.delete(`api/tasks/${guid}`);
}
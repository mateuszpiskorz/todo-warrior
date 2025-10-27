import * as signalR from '@microsoft/signalr';

export interface HubEvents {
    ReminderDue: (data: { guid: string, title: string, dueDate: Date }) => void;
}

const HUB_URL = import.meta.env.VITE_TASKS_HUB_URL ||
    (import.meta.env.VITE_API_BASE_URL ? 
    `${import.meta.env.VITE_API_BASE_URL}/hubs/reminders` :
    "http://localhost:8080/hubs/reminders");

export function buildConnection(){
    return new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL)
        .withAutomaticReconnect({ nextRetryDelayInMilliseconds: ctx => Math.min(1000 * (ctx.previousRetryCount+1), 15000) })
        .build();
}


export async function start(connection: signalR.HubConnection){
    if (connection.state === signalR.HubConnectionState.Connected) return;
    try { await connection.start(); console.log("SignalR connection started"); }
    catch (err){ console.error("SignalR start error", err); }
}


export function wireHandlers(connection: signalR.HubConnection, handlers: Partial<HubEvents>){
    if (handlers.ReminderDue) connection.on("Reminder", handlers.ReminderDue);
}


export function dispose(connection: signalR.HubConnection){
    try { connection.stop(); } catch {}
}

export type TaskItem = {
    guid: string;
    title: string;
    description?: string;
    isDone: boolean;
    dueDate?: string | null;
    reminderAt?: string | null;
    createdAt: string;
    updatedAt?: string;
    isActive: boolean;
}

export type NewTask = Omit<TaskItem, 
'guid' | 'createdAt' | 'updatedAt' | 'isActive' | 'isDone'>;

export type UpdateTask = Partial<Omit<TaskItem, 
'guid' | 'createdAt' | 'updatedAt' | 'isActive'> & { guid: string }>;
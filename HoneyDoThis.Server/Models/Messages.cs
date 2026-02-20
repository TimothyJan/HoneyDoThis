namespace HoneyDoThis.Server.Models
{
    public static class Messages
    {
        public static class Task
        {
            // Success messages
            public const string TaskCreated = "Task created successfully";
            public const string TaskUpdated = "Task updated successfully";
            public const string TaskDeleted = "Task deleted successfully";
            public const string TasksReordered = "Tasks reordered successfully";
            public const string TasksCleared = "Completed tasks cleared successfully";

            // Error messages
            public const string TaskNotFound = "Task not found";
            public const string TasksNotFound = "No tasks found";
            public const string InvalidTaskData = "Invalid task data provided";
            public const string TaskCreationFailed = "Failed to create task";
            public const string TaskUpdateFailed = "Failed to update task";
            public const string TaskDeleteFailed = "Failed to delete task";
            public const string TaskReorderFailed = "Failed to reorder tasks";
        }

        public static class Subtask
        {
            // Success messages
            public const string SubtaskCreated = "Subtask created successfully";
            public const string SubtaskUpdated = "Subtask updated successfully";
            public const string SubtaskDeleted = "Subtask deleted successfully";
            public const string SubtasksReordered = "Subtasks reordered successfully";
            public const string CompletedSubtasksCleared = "Completed subtasks cleared successfully";

            // Error messages
            public const string SubtaskNotFound = "Subtask not found";
            public const string SubtasksNotFound = "No subtasks found";
            public const string InvalidSubtaskData = "Invalid subtask data provided";
            public const string SubtaskCreationFailed = "Failed to create subtask";
            public const string SubtaskUpdateFailed = "Failed to update subtask";
            public const string SubtaskDeleteFailed = "Failed to delete subtask";
            public const string SubtaskReorderFailed = "Failed to reorder subtasks";
            public const string ParentTaskNotFound = "Parent task not found";
        }

        public static class General
        {
            public const string InternalServerError = "An internal server error occurred";
            public const string InvalidRequest = "Invalid request";
            public const string DatabaseError = "A database error occurred";
            public const string ValidationError = "Validation error";
        }
    }
}
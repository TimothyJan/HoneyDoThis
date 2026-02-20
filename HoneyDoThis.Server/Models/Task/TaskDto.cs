using System.ComponentModel.DataAnnotations;

namespace HoneyDoThis.Server.Models.Task
{
    public class TaskDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public bool Completed { get; set; }

        public int Order { get; set; }

        public bool Expanded { get; set; }

        public int? SubtaskCount { get; set; }

        public int? CompletedSubtaskCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public int Order { get; set; } = -1;
    }

    public class UpdateTaskDto
    {
        [MaxLength(500)]
        public string? Text { get; set; }

        public bool? Completed { get; set; }

        public bool? Expanded { get; set; }

        public int? Order { get; set; }
    }

    public class ReorderTasksDto
    {
        public int PreviousIndex { get; set; }
        public int CurrentIndex { get; set; }
    }
}
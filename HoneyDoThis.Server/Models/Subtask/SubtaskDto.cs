using System.ComponentModel.DataAnnotations;

namespace HoneyDoThis.Server.Models.Subtask
{
    public class SubtaskDto
    {
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public bool Completed { get; set; }

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSubtaskDto
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public int Order { get; set; } = -1;
    }

    public class UpdateSubtaskDto
    {
        [MaxLength(500)]
        public string? Text { get; set; }

        public bool? Completed { get; set; }

        public int? Order { get; set; }
    }

    public class ReorderSubtasksDto
    {
        public int TaskId { get; set; }
        public int PreviousIndex { get; set; }
        public int CurrentIndex { get; set; }
    }
}
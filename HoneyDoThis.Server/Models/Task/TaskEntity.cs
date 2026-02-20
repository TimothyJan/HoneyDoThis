using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoneyDoThis.Server.Models.Task
{
    [Table("Tasks")]
    public class TaskEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public bool Completed { get; set; }

        public int Order { get; set; }

        public bool Expanded { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Subtask.SubtaskEntity> Subtasks { get; set; } = new List<Subtask.SubtaskEntity>();
    }
}
using Microsoft.EntityFrameworkCore;
using HoneyDoThis.Server.Models.Task;
using HoneyDoThis.Server.Models.Subtask;

namespace HoneyDoThis.Server.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<SubtaskEntity> Subtasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Task configuration
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Subtasks)
                    .WithOne(s => s.Task)
                    .HasForeignKey(s => s.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Subtask configuration
            modelBuilder.Entity<SubtaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Task)
                    .WithMany(t => t.Subtasks)
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Create indexes for better performance
            modelBuilder.Entity<TaskEntity>()
                .HasIndex(e => e.Order);

            modelBuilder.Entity<SubtaskEntity>()
                .HasIndex(e => new { e.TaskId, e.Order });

            modelBuilder.Entity<SubtaskEntity>()
                .HasIndex(e => e.TaskId);
        }
    }
}
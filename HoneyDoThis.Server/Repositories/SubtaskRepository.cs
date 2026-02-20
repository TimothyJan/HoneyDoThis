using Microsoft.EntityFrameworkCore;
using HoneyDoThis.Server.Models;
using HoneyDoThis.Server.Models.Subtask;
using HoneyDoThis.Server.Repositories.Interfaces;

namespace HoneyDoThis.Server.Repositories
{
    public class SubtaskRepository : ISubtaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SubtaskRepository> _logger;

        public SubtaskRepository(ApplicationDbContext context, ILogger<SubtaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SubtaskEntity>> GetAllAsync()
        {
            return await _context.Subtasks
                .OrderBy(s => s.TaskId)
                .ThenBy(s => s.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<SubtaskEntity>> GetByTaskIdAsync(int taskId)
        {
            return await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .OrderBy(s => s.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SubtaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Subtasks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<int> GetCountByTaskIdAsync(int taskId)
        {
            return await _context.Subtasks
                .CountAsync(s => s.TaskId == taskId);
        }

        public async Task<int> GetCompletedCountByTaskIdAsync(int taskId)
        {
            return await _context.Subtasks
                .CountAsync(s => s.TaskId == taskId && s.Completed);
        }

        public async Task<bool> AreAllSubtasksCompletedAsync(int taskId)
        {
            var subtasks = await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .ToListAsync();

            return subtasks.Any() && subtasks.All(s => s.Completed);
        }

        public async Task<bool> TaskHasSubtasksAsync(int taskId)
        {
            return await _context.Subtasks
                .AnyAsync(s => s.TaskId == taskId);
        }

        public async Task<SubtaskEntity> CreateAsync(SubtaskEntity subtask)
        {
            // Verify parent task exists
            var taskExists = await _context.Tasks.AnyAsync(t => t.Id == subtask.TaskId);
            if (!taskExists)
            {
                throw new InvalidOperationException($"Task with ID {subtask.TaskId} does not exist");
            }

            if (subtask.Order < 0)
            {
                subtask.Order = await GetMaxOrderAsync(subtask.TaskId) + 1;
            }

            subtask.CreatedAt = DateTime.UtcNow;

            await _context.Subtasks.AddAsync(subtask);
            await _context.SaveChangesAsync();

            return subtask;
        }

        public async Task<SubtaskEntity?> UpdateAsync(SubtaskEntity subtask)
        {
            var existingSubtask = await _context.Subtasks.FindAsync(subtask.Id);
            if (existingSubtask == null)
            {
                return null;
            }

            subtask.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existingSubtask).CurrentValues.SetValues(subtask);
            _context.Entry(existingSubtask).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(existingSubtask).Property(x => x.TaskId).IsModified = false;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(subtask.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null)
            {
                return false;
            }

            var taskId = subtask.TaskId;

            _context.Subtasks.Remove(subtask);
            await _context.SaveChangesAsync();

            // Reorder remaining subtasks for this task
            await ReorderAfterDelete(taskId);

            return true;
        }

        public async Task<bool> DeleteByTaskIdAsync(int taskId)
        {
            var subtasks = await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .ToListAsync();

            if (!subtasks.Any())
            {
                return true; // Nothing to delete
            }

            _context.Subtasks.RemoveRange(subtasks);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCompletedByTaskIdAsync(int taskId)
        {
            var completedSubtasks = await _context.Subtasks
                .Where(s => s.TaskId == taskId && s.Completed)
                .ToListAsync();

            if (!completedSubtasks.Any())
            {
                return true; // Nothing to delete
            }

            _context.Subtasks.RemoveRange(completedSubtasks);
            await _context.SaveChangesAsync();

            // Reorder remaining subtasks
            await ReorderAfterDelete(taskId);

            return true;
        }

        public async Task<bool> ReorderAsync(int taskId, int previousIndex, int currentIndex)
        {
            if (previousIndex == currentIndex)
            {
                return true;
            }

            var subtasks = await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            if (previousIndex < 0 || previousIndex >= subtasks.Count ||
                currentIndex < 0 || currentIndex >= subtasks.Count)
            {
                return false;
            }

            var movedSubtask = subtasks[previousIndex];
            subtasks.RemoveAt(previousIndex);
            subtasks.Insert(currentIndex, movedSubtask);

            // Update order for all subtasks
            for (int i = 0; i < subtasks.Count; i++)
            {
                subtasks[i].Order = i;
                subtasks[i].UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SubtaskExistsAsync(int id)
        {
            return await _context.Subtasks.AnyAsync(s => s.Id == id);
        }

        public async Task<int> GetMaxOrderAsync(int taskId)
        {
            if (!await _context.Subtasks.AnyAsync(s => s.TaskId == taskId))
            {
                return -1;
            }

            return await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .MaxAsync(s => s.Order);
        }

        private async Task ReorderAfterDelete(int taskId)
        {
            var subtasks = await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            for (int i = 0; i < subtasks.Count; i++)
            {
                subtasks[i].Order = i;
            }

            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using HoneyDoThis.Server.Models;
using HoneyDoThis.Server.Models.Task;
using HoneyDoThis.Server.Repositories.Interfaces;

namespace HoneyDoThis.Server.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .OrderBy(t => t.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskEntity?> GetByIdWithSubtasksAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Subtasks.OrderBy(s => s.Order))
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskEntity>> GetFilteredAsync(bool? completed = null)
        {
            var query = _context.Tasks.AsQueryable();

            if (completed.HasValue)
            {
                query = query.Where(t => t.Completed == completed.Value);
            }

            return await query
                .OrderBy(t => t.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.Tasks
                .CountAsync(t => !t.Completed);
        }

        public async Task<int> GetCompletedCountAsync()
        {
            return await _context.Tasks
                .CountAsync(t => t.Completed);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Tasks.CountAsync();
        }

        public async Task<TaskEntity> CreateAsync(TaskEntity task)
        {
            if (task.Order < 0)
            {
                task.Order = await GetMaxOrderAsync() + 1;
            }

            task.CreatedAt = DateTime.UtcNow;

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<TaskEntity?> UpdateAsync(TaskEntity task)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);
            if (existingTask == null)
            {
                return null;
            }

            task.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existingTask).CurrentValues.SetValues(task);
            _context.Entry(existingTask).Property(x => x.CreatedAt).IsModified = false;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(task.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            // Reorder remaining tasks
            await ReorderAfterDelete();

            return true;
        }

        public async Task<bool> DeleteCompletedAsync()
        {
            var completedTasks = await _context.Tasks
                .Where(t => t.Completed)
                .ToListAsync();

            if (!completedTasks.Any())
            {
                return true; // Nothing to delete
            }

            _context.Tasks.RemoveRange(completedTasks);
            await _context.SaveChangesAsync();

            // Reorder remaining tasks
            await ReorderAfterDelete();

            return true;
        }

        public async Task<bool> ReorderAsync(int previousIndex, int currentIndex)
        {
            if (previousIndex == currentIndex)
            {
                return true;
            }

            var tasks = await _context.Tasks
                .OrderBy(t => t.Order)
                .ToListAsync();

            if (previousIndex < 0 || previousIndex >= tasks.Count ||
                currentIndex < 0 || currentIndex >= tasks.Count)
            {
                return false;
            }

            var movedTask = tasks[previousIndex];
            tasks.RemoveAt(previousIndex);
            tasks.Insert(currentIndex, movedTask);

            // Update order for all tasks
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Order = i;
                tasks[i].UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TaskExistsAsync(int id)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }

        public async Task<int> GetMaxOrderAsync()
        {
            if (!await _context.Tasks.AnyAsync())
            {
                return -1;
            }

            return await _context.Tasks.MaxAsync(t => t.Order);
        }

        private async Task ReorderAfterDelete()
        {
            var tasks = await _context.Tasks
                .OrderBy(t => t.Order)
                .ToListAsync();

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Order = i;
            }

            await _context.SaveChangesAsync();
        }
    }
}
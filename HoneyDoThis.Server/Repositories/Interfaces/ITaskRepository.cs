using HoneyDoThis.Server.Models.Task;

namespace HoneyDoThis.Server.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        // Query methods
        Task<IEnumerable<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<TaskEntity?> GetByIdWithSubtasksAsync(int id);
        Task<IEnumerable<TaskEntity>> GetFilteredAsync(bool? completed = null);
        Task<int> GetActiveCountAsync();
        Task<int> GetCompletedCountAsync();
        Task<int> GetTotalCountAsync();

        // Command methods
        Task<TaskEntity> CreateAsync(TaskEntity task);
        Task<TaskEntity?> UpdateAsync(TaskEntity task);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteCompletedAsync();
        Task<bool> ReorderAsync(int previousIndex, int currentIndex);
        Task<bool> TaskExistsAsync(int id);
        Task<int> GetMaxOrderAsync();
    }
}
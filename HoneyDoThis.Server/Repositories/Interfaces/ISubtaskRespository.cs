using HoneyDoThis.Server.Models.Subtask;

namespace HoneyDoThis.Server.Repositories.Interfaces
{
    public interface ISubtaskRepository
    {
        // Query methods
        Task<IEnumerable<SubtaskEntity>> GetAllAsync();
        Task<IEnumerable<SubtaskEntity>> GetByTaskIdAsync(int taskId);
        Task<SubtaskEntity?> GetByIdAsync(int id);
        Task<int> GetCountByTaskIdAsync(int taskId);
        Task<int> GetCompletedCountByTaskIdAsync(int taskId);
        Task<bool> AreAllSubtasksCompletedAsync(int taskId);
        Task<bool> TaskHasSubtasksAsync(int taskId);

        // Command methods
        Task<SubtaskEntity> CreateAsync(SubtaskEntity subtask);
        Task<SubtaskEntity?> UpdateAsync(SubtaskEntity subtask);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByTaskIdAsync(int taskId);
        Task<bool> ClearCompletedByTaskIdAsync(int taskId);
        Task<bool> ReorderAsync(int taskId, int previousIndex, int currentIndex);
        Task<bool> SubtaskExistsAsync(int id);
        Task<int> GetMaxOrderAsync(int taskId);
    }
}
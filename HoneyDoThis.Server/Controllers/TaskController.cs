using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HoneyDoThis.Server.Models;
using HoneyDoThis.Server.Models.Task;
using HoneyDoThis.Server.Repositories.Interfaces;

namespace HoneyDoThis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ISubtaskRepository _subtaskRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskRepository taskRepository,
            ISubtaskRepository subtaskRepository,
            IMapper mapper,
            ILogger<TaskController> logger)
        {
            _taskRepository = taskRepository;
            _subtaskRepository = subtaskRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/task
        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] bool? completed = null)
        {
            try
            {
                var tasks = completed.HasValue
                    ? await _taskRepository.GetFilteredAsync(completed)
                    : await _taskRepository.GetAllAsync();

                var taskDtos = await EnrichTasksWithSubtaskInfo(tasks);
                return Success(taskDtos, Messages.Task.TasksNotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // GET: api/task/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskRepository.GetByIdWithSubtasksAsync(id);
                if (task == null)
                {
                    return NotFound(Messages.Task.TaskNotFound);
                }

                var taskDto = _mapper.Map<TaskDto>(task);
                taskDto.SubtaskCount = task.Subtasks?.Count ?? 0;
                taskDto.CompletedSubtaskCount = task.Subtasks?.Count(s => s.Completed) ?? 0;

                return Success(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {Id}", id);
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // GET: api/task/counts
        [HttpGet("counts")]
        public async Task<IActionResult> GetTaskCounts()
        {
            try
            {
                var counts = new
                {
                    Active = await _taskRepository.GetActiveCountAsync(),
                    Completed = await _taskRepository.GetCompletedCountAsync(),
                    Total = await _taskRepository.GetTotalCountAsync()
                };

                return Success(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task counts");
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // POST: api/task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Messages.Task.InvalidTaskData, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                }

                var task = _mapper.Map<TaskEntity>(createDto);
                var created = await _taskRepository.CreateAsync(task);
                var taskDto = _mapper.Map<TaskDto>(created);

                return Created(taskDto, Messages.Task.TaskCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return InternalServerError(Messages.Task.TaskCreationFailed, ex);
            }
        }

        // PUT: api/task/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Messages.Task.InvalidTaskData, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                }

                var existingTask = await _taskRepository.GetByIdAsync(id);
                if (existingTask == null)
                {
                    return NotFound(Messages.Task.TaskNotFound);
                }

                _mapper.Map(updateDto, existingTask);
                var updated = await _taskRepository.UpdateAsync(existingTask);

                if (updated == null)
                {
                    return NotFound(Messages.Task.TaskNotFound);
                }

                var taskDto = _mapper.Map<TaskDto>(updated);
                return Success(taskDto, Messages.Task.TaskUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {Id}", id);
                return InternalServerError(Messages.Task.TaskUpdateFailed, ex);
            }
        }

        // DELETE: api/task/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var exists = await _taskRepository.TaskExistsAsync(id);
                if (!exists)
                {
                    return NotFound(Messages.Task.TaskNotFound);
                }

                var result = await _taskRepository.DeleteAsync(id);
                if (!result)
                {
                    return InternalServerError(Messages.Task.TaskDeleteFailed);
                }

                return Success<object>(null!, Messages.Task.TaskDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {Id}", id);
                return InternalServerError(Messages.Task.TaskDeleteFailed, ex);
            }
        }

        // POST: api/task/reorder
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderTasks([FromBody] ReorderTasksDto reorderDto)
        {
            try
            {
                var result = await _taskRepository.ReorderAsync(reorderDto.PreviousIndex, reorderDto.CurrentIndex);
                if (!result)
                {
                    return BadRequest(Messages.Task.TaskReorderFailed);
                }

                return Success<object>(null!, Messages.Task.TasksReordered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering tasks");
                return InternalServerError(Messages.Task.TaskReorderFailed, ex);
            }
        }

        // DELETE: api/task/completed/clear
        [HttpDelete("completed/clear")]
        public async Task<IActionResult> ClearCompleted()
        {
            try
            {
                await _taskRepository.DeleteCompletedAsync();
                return Success<object>(null!, Messages.Task.TasksCleared);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing completed tasks");
                return InternalServerError(Messages.Task.TaskDeleteFailed, ex);
            }
        }

        // GET: api/task/{id}/has-subtasks
        [HttpGet("{id}/has-subtasks")]
        public async Task<IActionResult> TaskHasSubtasks(int id)
        {
            try
            {
                var exists = await _taskRepository.TaskExistsAsync(id);
                if (!exists)
                {
                    return NotFound(Messages.Task.TaskNotFound);
                }

                var hasSubtasks = await _subtaskRepository.TaskHasSubtasksAsync(id);
                return Success(new { HasSubtasks = hasSubtasks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subtasks for task {Id}", id);
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        private async Task<IEnumerable<TaskDto>> EnrichTasksWithSubtaskInfo(IEnumerable<TaskEntity> tasks)
        {
            var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            foreach (var dto in taskDtos)
            {
                dto.SubtaskCount = await _subtaskRepository.GetCountByTaskIdAsync(dto.Id);
                dto.CompletedSubtaskCount = await _subtaskRepository.GetCompletedCountByTaskIdAsync(dto.Id);
            }

            return taskDtos;
        }
    }
}
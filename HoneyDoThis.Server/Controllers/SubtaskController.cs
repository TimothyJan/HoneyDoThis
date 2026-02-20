using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HoneyDoThis.Server.Models;
using HoneyDoThis.Server.Models.Subtask;
using HoneyDoThis.Server.Repositories.Interfaces;

namespace HoneyDoThis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubtaskController : BaseController
    {
        private readonly ISubtaskRepository _subtaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SubtaskController> _logger;

        public SubtaskController(
            ISubtaskRepository subtaskRepository,
            ITaskRepository taskRepository,
            IMapper mapper,
            ILogger<SubtaskController> logger)
        {
            _subtaskRepository = subtaskRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/subtask
        [HttpGet]
        public async Task<IActionResult> GetAllSubtasks()
        {
            try
            {
                var subtasks = await _subtaskRepository.GetAllAsync();
                var subtaskDtos = _mapper.Map<IEnumerable<SubtaskDto>>(subtasks);
                return Success(subtaskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subtasks");
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // GET: api/subtask/task/{taskId}
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetSubtasksByTaskId(int taskId)
        {
            try
            {
                var taskExists = await _taskRepository.TaskExistsAsync(taskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var subtasks = await _subtaskRepository.GetByTaskIdAsync(taskId);
                var subtaskDtos = _mapper.Map<IEnumerable<SubtaskDto>>(subtasks);
                return Success(subtaskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subtasks for task {TaskId}", taskId);
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // GET: api/subtask/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubtaskById(int id)
        {
            try
            {
                var subtask = await _subtaskRepository.GetByIdAsync(id);
                if (subtask == null)
                {
                    return NotFound(Messages.Subtask.SubtaskNotFound);
                }

                var subtaskDto = _mapper.Map<SubtaskDto>(subtask);
                return Success(subtaskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subtask {Id}", id);
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // GET: api/subtask/task/{taskId}/counts
        [HttpGet("task/{taskId}/counts")]
        public async Task<IActionResult> GetSubtaskCounts(int taskId)
        {
            try
            {
                var taskExists = await _taskRepository.TaskExistsAsync(taskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var counts = new
                {
                    Total = await _subtaskRepository.GetCountByTaskIdAsync(taskId),
                    Completed = await _subtaskRepository.GetCompletedCountByTaskIdAsync(taskId),
                    AllCompleted = await _subtaskRepository.AreAllSubtasksCompletedAsync(taskId)
                };

                return Success(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subtask counts for task {TaskId}", taskId);
                return InternalServerError(Messages.General.InternalServerError, ex);
            }
        }

        // POST: api/subtask
        [HttpPost]
        public async Task<IActionResult> CreateSubtask([FromBody] CreateSubtaskDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Messages.Subtask.InvalidSubtaskData, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                }

                var taskExists = await _taskRepository.TaskExistsAsync(createDto.TaskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var subtask = _mapper.Map<SubtaskEntity>(createDto);
                var created = await _subtaskRepository.CreateAsync(subtask);
                var subtaskDto = _mapper.Map<SubtaskDto>(created);

                return Created(subtaskDto, Messages.Subtask.SubtaskCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subtask");
                return InternalServerError(Messages.Subtask.SubtaskCreationFailed, ex);
            }
        }

        // PUT: api/subtask/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubtask(int id, [FromBody] UpdateSubtaskDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Messages.Subtask.InvalidSubtaskData, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                }

                var existingSubtask = await _subtaskRepository.GetByIdAsync(id);
                if (existingSubtask == null)
                {
                    return NotFound(Messages.Subtask.SubtaskNotFound);
                }

                _mapper.Map(updateDto, existingSubtask);
                var updated = await _subtaskRepository.UpdateAsync(existingSubtask);

                if (updated == null)
                {
                    return NotFound(Messages.Subtask.SubtaskNotFound);
                }

                var subtaskDto = _mapper.Map<SubtaskDto>(updated);
                return Success(subtaskDto, Messages.Subtask.SubtaskUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subtask {Id}", id);
                return InternalServerError(Messages.Subtask.SubtaskUpdateFailed, ex);
            }
        }

        // DELETE: api/subtask/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubtask(int id)
        {
            try
            {
                var exists = await _subtaskRepository.SubtaskExistsAsync(id);
                if (!exists)
                {
                    return NotFound(Messages.Subtask.SubtaskNotFound);
                }

                var result = await _subtaskRepository.DeleteAsync(id);
                if (!result)
                {
                    return InternalServerError(Messages.Subtask.SubtaskDeleteFailed);
                }

                return Success<object>(null!, Messages.Subtask.SubtaskDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subtask {Id}", id);
                return InternalServerError(Messages.Subtask.SubtaskDeleteFailed, ex);
            }
        }

        // DELETE: api/subtask/task/{taskId}
        [HttpDelete("task/{taskId}")]
        public async Task<IActionResult> DeleteSubtasksByTaskId(int taskId)
        {
            try
            {
                var taskExists = await _taskRepository.TaskExistsAsync(taskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var result = await _subtaskRepository.DeleteByTaskIdAsync(taskId);
                if (!result)
                {
                    return InternalServerError(Messages.Subtask.SubtaskDeleteFailed);
                }

                return Success<object>(null!, Messages.Subtask.SubtaskDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subtasks for task {TaskId}", taskId);
                return InternalServerError(Messages.Subtask.SubtaskDeleteFailed, ex);
            }
        }

        // POST: api/subtask/reorder
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderSubtasks([FromBody] ReorderSubtasksDto reorderDto)
        {
            try
            {
                var taskExists = await _taskRepository.TaskExistsAsync(reorderDto.TaskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var result = await _subtaskRepository.ReorderAsync(
                    reorderDto.TaskId,
                    reorderDto.PreviousIndex,
                    reorderDto.CurrentIndex);

                if (!result)
                {
                    return BadRequest(Messages.Subtask.SubtaskReorderFailed);
                }

                return Success<object>(null!, Messages.Subtask.SubtasksReordered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering subtasks");
                return InternalServerError(Messages.Subtask.SubtaskReorderFailed, ex);
            }
        }

        // DELETE: api/subtask/task/{taskId}/completed
        [HttpDelete("task/{taskId}/completed")]
        public async Task<IActionResult> ClearCompletedSubtasks(int taskId)
        {
            try
            {
                var taskExists = await _taskRepository.TaskExistsAsync(taskId);
                if (!taskExists)
                {
                    return NotFound(Messages.Subtask.ParentTaskNotFound);
                }

                var result = await _subtaskRepository.ClearCompletedByTaskIdAsync(taskId);
                if (!result)
                {
                    return InternalServerError(Messages.Subtask.SubtaskDeleteFailed);
                }

                return Success<object>(null!, Messages.Subtask.CompletedSubtasksCleared);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing completed subtasks for task {TaskId}", taskId);
                return InternalServerError(Messages.Subtask.SubtaskDeleteFailed, ex);
            }
        }
    }
}
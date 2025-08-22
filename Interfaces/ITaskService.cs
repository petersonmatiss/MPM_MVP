using MPM_MVP.Models;
using MPM_MVP.DTOs;

namespace MPM_MVP.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(int projectId, CreateTaskDto dto);
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetProjectTasksAsync(int projectId);
    Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId);
    Task<TaskItem> UpdateTaskStatusAsync(int id, Models.TaskStatus status);
    Task<TaskItem> AssignTaskAsync(int id, int? assigneeId);
    Task DeleteTaskAsync(int id);
}
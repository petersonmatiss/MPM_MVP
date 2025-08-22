using Microsoft.EntityFrameworkCore;
using MPM_MVP.Models;
using MPM_MVP.DTOs;
using MPM_MVP.Interfaces;
using MPM_MVP.Data;

namespace MPM_MVP.Services;

public class TaskService : ITaskService
{
    private readonly MPMDbContext _context;
    
    public TaskService(MPMDbContext context)
    {
        _context = context;
    }
    
    public async Task<TaskItem> CreateTaskAsync(int projectId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            AssigneeId = dto.AssigneeId,
            ProjectId = projectId
        };
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }
    
    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task<IEnumerable<TaskItem>> GetProjectTasksAsync(int projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Assignee)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId)
    {
        return await _context.Tasks
            .Where(t => t.AssigneeId == userId)
            .Include(t => t.Project)
            .ToListAsync();
    }
    
    public async Task<TaskItem> UpdateTaskStatusAsync(int id, Models.TaskStatus status)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) throw new ArgumentException("Task not found");
        
        task.Status = status;
        if (status == Models.TaskStatus.Done && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        return task;
    }
    
    public async Task<TaskItem> AssignTaskAsync(int id, int? assigneeId)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) throw new ArgumentException("Task not found");
        
        task.AssigneeId = assigneeId;
        await _context.SaveChangesAsync();
        return task;
    }
    
    public async Task DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
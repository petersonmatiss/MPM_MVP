using Microsoft.AspNetCore.Mvc;
using MPM_MVP.Models;
using MPM_MVP.DTOs;
using MPM_MVP.Interfaces;

namespace MPM_MVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }
    
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetProjectTasks(int projectId)
    {
        var tasks = await _taskService.GetProjectTasksAsync(projectId);
        return Ok(tasks);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetUserTasks(int userId)
    {
        var tasks = await _taskService.GetUserTasksAsync(userId);
        return Ok(tasks);
    }
    
    [HttpPost("project/{projectId}")]
    public async Task<ActionResult<TaskItem>> CreateTask(int projectId, CreateTaskDto dto)
    {
        var task = await _taskService.CreateTaskAsync(projectId, dto);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }
    
    [HttpPut("{id}/status")]
    public async Task<ActionResult<TaskItem>> UpdateTaskStatus(int id, [FromBody] Models.TaskStatus status)
    {
        try
        {
            var task = await _taskService.UpdateTaskStatusAsync(id, status);
            return Ok(task);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
    
    [HttpPut("{id}/assign")]
    public async Task<ActionResult<TaskItem>> AssignTask(int id, [FromQuery] int? assigneeId)
    {
        try
        {
            var task = await _taskService.AssignTaskAsync(id, assigneeId);
            return Ok(task);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}
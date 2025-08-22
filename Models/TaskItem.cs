using System.ComponentModel.DataAnnotations;

namespace MPM_MVP.Models;

public class TaskItem
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    
    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }
}
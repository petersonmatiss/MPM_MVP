using System.ComponentModel.DataAnnotations;
using MPM_MVP.Models;

namespace MPM_MVP.DTOs;

public class CreateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public int? AssigneeId { get; set; }
}
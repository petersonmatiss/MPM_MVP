using System.ComponentModel.DataAnnotations;

namespace MPM_MVP.Models;

public class Project
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    public List<TaskItem> Tasks { get; set; } = new();
    public List<ProjectMember> Members { get; set; } = new();
}
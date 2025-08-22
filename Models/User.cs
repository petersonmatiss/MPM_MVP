using System.ComponentModel.DataAnnotations;

namespace MPM_MVP.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Project> Projects { get; set; } = new();
    public List<TaskItem> AssignedTasks { get; set; } = new();
}
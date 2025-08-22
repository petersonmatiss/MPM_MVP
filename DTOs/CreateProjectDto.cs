using System.ComponentModel.DataAnnotations;

namespace MPM_MVP.DTOs;

public class CreateProjectDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
using Microsoft.AspNetCore.Mvc;
using MPM_MVP.Models;
using MPM_MVP.DTOs;
using MPM_MVP.Interfaces;

namespace MPM_MVP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null) return NotFound();
        return Ok(project);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects(int userId)
    {
        var projects = await _projectService.GetUserProjectsAsync(userId);
        return Ok(projects);
    }
    
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(CreateProjectDto dto, [FromQuery] int ownerId)
    {
        var project = await _projectService.CreateProjectAsync(dto, ownerId);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Project>> UpdateProject(int id, CreateProjectDto dto)
    {
        try
        {
            var project = await _projectService.UpdateProjectAsync(id, dto);
            return Ok(project);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await _projectService.DeleteProjectAsync(id);
        return NoContent();
    }
    
    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, [FromQuery] int userId, [FromQuery] string role = "Member")
    {
        await _projectService.AddMemberAsync(id, userId, role);
        return Ok();
    }
    
    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int id, int userId)
    {
        await _projectService.RemoveMemberAsync(id, userId);
        return NoContent();
    }
}
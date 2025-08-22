using Microsoft.EntityFrameworkCore;
using MPM_MVP.Models;
using MPM_MVP.DTOs;
using MPM_MVP.Interfaces;
using MPM_MVP.Data;

namespace MPM_MVP.Services;

public class ProjectService : IProjectService
{
    private readonly MPMDbContext _context;
    
    public ProjectService(MPMDbContext context)
    {
        _context = context;
    }
    
    public async Task<Project> CreateProjectAsync(CreateProjectDto dto, int ownerId)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            OwnerId = ownerId
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }
    
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
    {
        return await _context.Projects
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Owner)
            .ToListAsync();
    }
    
    public async Task<Project> UpdateProjectAsync(int id, CreateProjectDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) throw new ArgumentException("Project not found");
        
        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        
        await _context.SaveChangesAsync();
        return project;
    }
    
    public async Task DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddMemberAsync(int projectId, int userId, string role = "Member")
    {
        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role
        };
        
        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemoveMemberAsync(int projectId, int userId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
        
        if (member != null)
        {
            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
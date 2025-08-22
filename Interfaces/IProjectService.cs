using MPM_MVP.Models;
using MPM_MVP.DTOs;

namespace MPM_MVP.Interfaces;

public interface IProjectService
{
    Task<Project> CreateProjectAsync(CreateProjectDto dto, int ownerId);
    Task<Project?> GetProjectByIdAsync(int id);
    Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
    Task<Project> UpdateProjectAsync(int id, CreateProjectDto dto);
    Task DeleteProjectAsync(int id);
    Task AddMemberAsync(int projectId, int userId, string role = "Member");
    Task RemoveMemberAsync(int projectId, int userId);
}
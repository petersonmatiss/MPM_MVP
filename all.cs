using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MPM_MVP;

// =================== MODELS ===================

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

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Role { get; set; } = "Member";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

// =================== ENUMS ===================

public enum ProjectStatus
{
    Planning,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum TaskStatus
{
    ToDo,
    InProgress,
    InReview,
    Done,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

// =================== DTOs ===================

public class CreateProjectDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

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

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string Role { get; set; } = "User";
}

// =================== INTERFACES ===================

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<User?> GetUserByIdAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(int id, CreateUserDto dto);
    Task DeleteUserAsync(int id);
}

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

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(int projectId, CreateTaskDto dto);
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetProjectTasksAsync(int projectId);
    Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId);
    Task<TaskItem> UpdateTaskStatusAsync(int id, TaskStatus status);
    Task<TaskItem> AssignTaskAsync(int id, int? assigneeId);
    Task DeleteTaskAsync(int id);
}

// =================== DATA CONTEXT ===================

public class MPMDbContext : DbContext
{
    public MPMDbContext(DbContextOptions<MPMDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany()
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<ProjectMember>()
            .HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();
    }
}

// =================== SERVICES ===================

public class UserService : IUserService
{
    private readonly MPMDbContext _context;
    
    public UserService(MPMDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<User> UpdateUserAsync(int id, CreateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new ArgumentException("User not found");
        
        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = dto.Role;
        
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

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
    
    public async Task<TaskItem> UpdateTaskStatusAsync(int id, TaskStatus status)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) throw new ArgumentException("Task not found");
        
        task.Status = status;
        if (status == TaskStatus.Done && task.CompletedAt == null)
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

// =================== CONTROLLERS ===================

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, CreateUserDto dto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}

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
    public async Task<ActionResult<TaskItem>> UpdateTaskStatus(int id, [FromBody] TaskStatus status)
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

// =================== PROGRAM (STARTUP) ===================

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services
        builder.Services.AddControllers();
        builder.Services.AddDbContext<MPMDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // Register services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        
        // Add Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        
        // Configure pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}
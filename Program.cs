using Microsoft.EntityFrameworkCore;
using MPM_MVP.Data;
using MPM_MVP.Interfaces;
using MPM_MVP.Services;

namespace MPM_MVP;

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
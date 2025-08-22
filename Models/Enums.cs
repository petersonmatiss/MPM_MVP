namespace MPM_MVP.Models;

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
namespace NBL_PoC_Api.Todos;

public sealed class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool IsCompleted { get; set; }
}
public sealed class TodoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool IsCompleted { get; set; }
}

public static class TodoMappingExtensions
{
    public static TodoDto AsTodoDto(this Todo todo)
    {
        return new()
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted,
            Description = todo.Description,
        };
    }
}
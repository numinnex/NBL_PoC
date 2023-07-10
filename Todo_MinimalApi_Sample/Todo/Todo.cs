namespace Todo_MinimalApi_Sample.Todo;

public sealed class Todo
{
	public int Id { get; set; }
	public string Title { get; set; } = default!;
	public string Description { get; set; } = default!;
	public bool IsCompleted { get; set; }
}
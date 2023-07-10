using Microsoft.EntityFrameworkCore;

namespace Todo_MinimalApi_Sample.Persistance;

public sealed class TodoDbContext : DbContext
{
	public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
	{
		
	}	
}
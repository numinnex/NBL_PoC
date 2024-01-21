using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NBL_PoC_Api.Seeder;
using NBL_PoC_Api.Todos;

namespace NBL_PoC_Api.Persistance;

public sealed class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoTableConfiguration());
        modelBuilder.HasDefaultSchema("todos");
        base.OnModelCreating(modelBuilder);
    }
}

file sealed class TodoTableConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x => x.IsCompleted).IsRequired();
        builder.HasKey(x => x.Id);
        builder.HasData(CreateTodos());
    }

    private static List<Todo> CreateTodos()
    {
        return
        [
            new Todo { Id = 1, Description = "Test_desc_1", Title = "Test_title_1", IsCompleted = true },
            new Todo { Id = 2, Description = "Test_desc_2", Title = "Test_title_2", IsCompleted = false },
            new Todo { Id = 3, Description = "Test_desc_3", Title = "Test_title_3", IsCompleted = true },
            new Todo { Id = 4, Description = "Test_desc_4", Title = "Test_title_4", IsCompleted = false },
            new Todo { Id = 5, Description = "Test_desc_5", Title = "Test_title_5", IsCompleted = false }
        ];
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    }
}
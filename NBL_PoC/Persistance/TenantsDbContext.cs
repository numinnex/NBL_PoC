using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NBL_PoC_Api.Tenants;

namespace NBL_PoC_Api.Persistance;

public class TenantsDbContext : DbContext
{
	public TenantsDbContext(DbContextOptions<TenantsDbContext> options) : base(options)
	{
        
	}
	public DbSet<Tenant> Tenants { get;set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new TenantsTableConfiguration());
        modelBuilder.HasDefaultSchema("tenants");
		base.OnModelCreating(modelBuilder);
	}
}
file sealed class TenantsTableConfiguration : IEntityTypeConfiguration<Tenant>
{
		public void Configure(EntityTypeBuilder<Tenant> builder)
	{
		builder.Property(x => x.ConnectionString).HasMaxLength(350).IsRequired();
		builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
		builder.HasKey(x => x.Id);
	}
}
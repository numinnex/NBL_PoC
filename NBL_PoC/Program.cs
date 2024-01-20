using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Crypto;
using NBL_PoC_Api.Options.AES;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Seeder;
using NBL_PoC_Api.Tenants;
using NBL_PoC_Api.Version;

var builder = WebApplication.CreateBuilder(args);

// Tenant setup
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenant>(sp =>
{
    var tenantIdString = sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request.Headers["TenantId"]; 
    return !string.IsNullOrEmpty(tenantIdString) && int.TryParse(tenantIdString, out var tenantId)
        ? new TenantData(tenantId)
        : null;
});
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddDbContext<TenantsDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("TenantsConnectionString"));
});

builder.Services.AddPooledDbContextFactory<TodoDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<TodoDbContextScopedFactory>();
builder.Services.AddScoped(sp 
	=> sp.GetRequiredService<TodoDbContextScopedFactory>().CreateDbContext());
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = ApiVersioning.V1;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

builder.Services.AddScoped<IEncryptor, AesEncryptor>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AesSettings>(builder.Configuration.GetSection("AES-Settings"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var tenantService = scope.ServiceProvider.GetService<ITenantService>();
	var todoContext = scope.ServiceProvider.GetService<TodoDbContext>();
	var dbContext = scope.ServiceProvider.GetService<TenantsDbContext>();
	var encryptor = scope.ServiceProvider.GetService<IEncryptor>();
	await dbContext!.Database.MigrateAsync();

	await DbSeeder.SeedTenantsAsync(5, dbContext, encryptor!);
	var tenantIds = await dbContext.Tenants.Select(x => x.Id).ToListAsync();
	foreach (var tenantId in tenantIds)
	{
		Console.WriteLine("tenant id: {0}", tenantId);
		var tenantCs = await tenantService!.GetConnectionStringAsync(tenantId);
		Console.WriteLine("db cs {0}",dbContext.Database.GetConnectionString());
		Console.WriteLine("tenant cs {0}", tenantCs);
		todoContext!.Database.SetConnectionString(tenantCs);
		await todoContext.Database.MigrateAsync();
	}
}


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
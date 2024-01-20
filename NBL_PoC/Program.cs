using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Tenant;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Todos;
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
	var dbContext = scope.ServiceProvider.GetService<TenantsDbContext>();
	var encryptor = scope.ServiceProvider.GetService<IEncryptor>();
	await dbContext!.Database.MigrateAsync();

	await DbSeeder.SeedTenantsAsync(5, dbContext, encryptor!);
}


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using NBL_PoC.Persistance;
using NBL_PoC.Tenant;
using NBL_PoC.Todos;
using NBL_PoC.Version;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetService<TodoDbContext>();
	await dbContext!.Database.MigrateAsync();
}


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapTodos();
app.UseHttpsRedirection();

app.Run();
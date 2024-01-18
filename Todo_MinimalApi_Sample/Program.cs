using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Todo_MinimalApi_Sample.Persistance;
using Todo_MinimalApi_Sample.Tenant;
using Todo_MinimalApi_Sample.Todos;
using Todo_MinimalApi_Sample.Version;

var builder = WebApplication.CreateBuilder(args);

// Tenant setup
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenant>(sp =>
{
    var tenantIdString = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request.Headers["TenantId"];
    return tenantIdString != StringValues.Empty && int.TryParse(tenantIdString, out var tenantId)
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
builder.Services.AddScoped(async sp 
	=> await sp.GetRequiredService<TodoDbContextScopedFactory>().CreateDbContextAsync());
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
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Todo_MinimalApi_Sample.Persistance;
using Todo_MinimalApi_Sample.Todos;
using Todo_MinimalApi_Sample.Version;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
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
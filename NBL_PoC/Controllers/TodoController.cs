using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBL_PoC_Api.Persistance;
using NBL_PoC_Api.Todos;

namespace NBL_PoC_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoDbContext _ctx;

    public TodoController(TodoDbContext ctx)
    {
        _ctx = ctx;
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute]int id, CancellationToken token)
    {
        return await _ctx.Todos.FindAsync([id], token) switch
        {
            Todo todo => Ok(todo.AsTodoDto()),
            _ => BadRequest()
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        return await _ctx.Todos.Select(x => x.AsTodoDto()).ToListAsync(token) switch 
        {
            List<TodoDto> todos => Ok(todos),
            _ => BadRequest()
        };
    }
    [HttpPost]
    public async Task<IActionResult> Create(TodoDto dto, CancellationToken token) 
    {
        var todo = new Todo
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
        };

        await _ctx.Todos.AddAsync(todo, token);
        await _ctx.SaveChangesAsync(token);

        return Created($"api/Todo/{todo.Id}", todo.AsTodoDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute]int id, TodoDto dto, CancellationToken token)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var rowsAffected = await _ctx.Todos.Where(x => x.Id == id)
            .ExecuteUpdateAsync(updates =>
                updates.SetProperty(t => t.Description, dto.Description)
                    .SetProperty(t => t.Title, dto.Title)
                    .SetProperty(t => t.IsCompleted, dto.IsCompleted), token);

        return rowsAffected == 0 ? NotFound() : NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken token) 
    {
        var rowsAffected = await _ctx.Todos.Where(x => x.Id == id)
            .ExecuteDeleteAsync(token);
        return rowsAffected == 0 ? NotFound() : NoContent();
    }
}
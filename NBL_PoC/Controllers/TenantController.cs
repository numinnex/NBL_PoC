using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token) 
    {
        return Ok(await _tenantService.GetAll(token));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken token) 
    {
        return await _tenantService.GetById(id, token) switch 
        {
            TenantDto dto => Ok(dto),
            _ => NotFound()
        };
    }


    [HttpPost]
    public async Task<IActionResult> Create(TenantDto dto, CancellationToken token) 
    {
        await _tenantService.CreateAsync(dto, token);
        // Do we want to return the created tenant in the response, together with connection string?
        return Created($"api/Tenant/{dto.Id}", dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromQuery]int id, CancellationToken token) 
    {
        var result = await _tenantService.DeleteAsync(id, token);
        return result ? NoContent() : NotFound();
    }
}
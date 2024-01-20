using Microsoft.AspNetCore.Mvc;
using NBL_PoC_Api.Tenants;

namespace NBL_PoC_Api.Controllers;

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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute]int id, CancellationToken token) 
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute]int id, CancellationToken token) 
    {
        var result = await _tenantService.DeleteAsync(id, token);
        return result ? NoContent() : NotFound();
    }
}
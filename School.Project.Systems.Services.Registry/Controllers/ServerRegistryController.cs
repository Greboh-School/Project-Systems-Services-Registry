using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Project.Systems.Services.Registry.Services;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Requests;

namespace School.Project.Systems.Services.Registry.Controllers;

[ApiController]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class ServerRegistryController : ControllerBase
{
    private readonly IServerRegistry _serverRegistry;
    private readonly ILogger<ServerRegistryController> _logger;

    public ServerRegistryController(IServerRegistry serverRegistry, ILogger<ServerRegistryController> logger)
    {
        _serverRegistry = serverRegistry;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [Authorize("game:user")]
    public async Task<ActionResult<ServerDTO>> Create([FromBody] ServerRegistrationRequest request)
    {
        var result = await _serverRegistry.Create(request);

        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ServerDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize("game:user")]
    public async Task<ActionResult<List<PlayerDTO>>> GetAll()
    {
        var result = await _serverRegistry.GetAll();
        
        return Ok(result);
    }    
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize("game:user")]
    public async Task<ActionResult<ServerDTO>> Get([FromRoute] Guid id)
    {
        var result = await _serverRegistry.Get(id);
        
        return Ok(result);
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteAll()
    {
        await _serverRegistry.DeleteAll();

        return Ok();
    } 
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _serverRegistry.Delete(id);

        return Ok();
    }
}
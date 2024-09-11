using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Project.Systems.Services.Registry.Services;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Requests;

namespace School.Project.Systems.Services.Registry.Controllers;

[ApiController]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class PlayerRegistryController : ControllerBase
{
    private readonly IPlayerRegistry _playerRegistry;
    private readonly ILogger<PlayerRegistryController> _logger;

    public PlayerRegistryController(IPlayerRegistry serverRegistry, ILogger<PlayerRegistryController> logger)
    {
        _playerRegistry = serverRegistry;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize("game:user")]
    public async Task<ActionResult<PlayerDTO>> Create([FromBody] PlayerConnectRequest request)
    {
        var result = await _playerRegistry.Create(request);

        return CreatedAtAction(nameof(Get), new { id = result.ServerId }, result);
    }
    
    [HttpPost("{serverId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize("game:user")]
    public async Task<ActionResult<PlayerDTO>> CreateWithServerId([FromRoute] Guid serverId, [FromBody] PlayerConnectRequest request)
    {
        var result = await _playerRegistry.CreateWithServerId(serverId, request);

        return CreatedAtAction(nameof(Get), new { id = result.ServerId }, result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PlayerDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize("game:user")]
    public async Task<ActionResult<List<PlayerDTO>>> GetAll()
    {
        var result = await _playerRegistry.GetAll();
        
        return Ok(result);
    }    
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize("game:user")]
    public async Task<ActionResult<PlayerDTO>> Get([FromRoute] Guid id)
    {
        var result = await _playerRegistry.Get(id);
        
        return Ok(result);
    }    
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize("game:user")]
    public async Task<ActionResult<PlayerDTO>> GetByUserName([FromRoute] string userName)
    {
        var result = await _playerRegistry.GetByUserName(userName);
        
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _playerRegistry.Delete(id);

        return Ok();
    }
}
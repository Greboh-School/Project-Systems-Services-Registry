using Microsoft.AspNetCore.Mvc;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Services;

namespace School.Project.Systems.Services.Registry.Controllers;

[ApiController]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class BrokerController : ControllerBase
{
    private readonly IBrokerService _service;
    private readonly ILogger<BrokerController> _logger;

    public BrokerController(IBrokerService service, ILogger<BrokerController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("queue/{serverId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateServerQueue([FromRoute] Guid serverId)
    {
        _service.CreateServerQueue(serverId);

        return Ok();
    }

    [HttpPost("message")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] MessageDTO dto)
    {
        _service.Create(dto);
        
        return Ok();
    }
}
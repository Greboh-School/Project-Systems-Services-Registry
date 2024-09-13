using Mapster;
using Microsoft.Extensions.Caching.Memory;
using School.Project.Systems.Services.Registry.Extensions;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Entities;
using School.Project.Systems.Services.Registry.Models.Requests;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Project.Systems.Services.Registry.Services;

public interface IServerRegistry
{
    public Task<ServerDTO> Create(ServerRegistrationRequest request);
    public Task<List<ServerDTO>> GetAll();
    public Task<ServerDTO> Get(Guid id);
    public Task DeleteAll();
    public Task Delete(Guid id);
}

public class ServerRegistry : IServerRegistry
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ServerRegistry> _logger;

    public ServerRegistry(IMemoryCache cache, ILogger<ServerRegistry> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<ServerDTO> Create(ServerRegistrationRequest request)
    {
        var entity = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = request.Address,
            Port = request.Port,
            ListenAddress = request.ListenAddress,
            Capacity = 2,
            Current = 0
        };

        // Get the existing "Servers" list from the cache, or create a new one if it doesn't exist
        var servers = await _cache.GetOrCreateAsync("SERVERS", _ => Task.FromResult(new List<ServerEntity>()));
        servers!.Add(entity);

        _cache.Set("SERVERS", servers);
        
        _logger.LogInformation("Server successfully registered with information {information}", entity.ToString());

        return new(entity.Id, entity.Address, entity.Port, entity.Capacity, entity.Current);
    }

    public Task<List<ServerDTO>> GetAll()
    {
        var entities = _cache.Get<List<ServerEntity>>(IMemoryCacheRepository.SERVER_KEY) ?? [];

        var dto = entities.Adapt<List<ServerDTO>>();

        return Task.FromResult(dto);
    }

    public Task<ServerDTO> Get(Guid id)
    {
        var entity = _cache.Get<List<ServerEntity>>("SERVERS")?.FirstOrDefault(x => x.Id == id);
        
        if (entity is null)
        {
            _logger.LogError("Failed to find server with id {id}", id);
            throw new NotFoundException($"Failed to find server with id {id}");
        }

        var dto = entity.Adapt<ServerDTO>();

        return Task.FromResult(dto);
    }

    public Task DeleteAll()
    {
        _cache.Remove(IMemoryCacheRepository.SERVER_KEY);

        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        await _cache.DeleteServer(id);
    }
}
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using School.Project.Systems.Services.Registry.Extensions;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Entities;
using School.Project.Systems.Services.Registry.Models.Requests;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Project.Systems.Services.Registry.Services;

public interface IPlayerRegistry
{
    public Task<PlayerDTO> Create(PlayerConnectRequest request);
    public Task<PlayerDTO> CreateWithServerId(Guid serverId, PlayerConnectRequest request);
    public Task<List<PlayerDTO>> GetAll();
    public PlayerDTO Get(Guid id);
    public Task<PlayerDTO> GetByUserName(string userName);
    public Task Delete(Guid id);
}

public class PlayerRegistry : IPlayerRegistry
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ServerRegistry> _logger;

    public PlayerRegistry(IMemoryCache cache, ILogger<ServerRegistry> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<PlayerDTO> Create(PlayerConnectRequest request)
    {
        var server = _cache.GetFirstAvailableServer();
        if (server is null)
        {
            _logger.LogError("Player tried to connect to a server but found none!");
            throw new NotFoundException("Failed to find any servers!");
        }

        var entity = new PlayerEntity(request.UserName, request.UserId, server.Id, server.Address, server.Port);
        var players = await _cache.GetOrCreatePlayers();
        
        players.Add(entity);

        _cache.Set(IMemoryCacheRepository.PLAYER_KEY, players);

        var dto = new PlayerDTO(entity.UserId, entity.UserName, entity.ServerId, server.Address, server.Port);
        
        _logger.LogInformation("Server connected {username} with id: {playerId} to server with ip: {serverIp}", entity.UserName, entity.UserId, entity.ServerId);
        
        return dto;
    }

    public async Task<PlayerDTO> CreateWithServerId(Guid serverId, PlayerConnectRequest request)
    {
        var server = _cache.GetServer(serverId);

        if (server is null)
        {
            _logger.LogError("{userId} tried to connect to server with id: {serverId} but it doesn't exist or is at full capacity!", request.UserId, serverId);
            throw new NotFoundException($"Failed to find server or its at full capacity with provided id: {serverId}");
        }

        var entity = new PlayerEntity(request.UserName, request.UserId, serverId, server.Address, server.Port);

        var players = await _cache.GetOrCreatePlayers();
        
        players.Add(entity);

        _cache.Set(IMemoryCacheRepository.PLAYER_KEY, players);

        var dto = entity.Adapt<PlayerDTO>();
        
        return dto;
    }

    public Task<List<PlayerDTO>> GetAll()
    {
        var entities = _cache.Get<List<PlayerEntity>>(IMemoryCacheRepository.PLAYER_KEY) ?? [];

        var dto = entities.Adapt<List<PlayerDTO>>();

        return Task.FromResult(dto);
    }


    public PlayerDTO Get(Guid id)
    {
        var entity = _cache.Get<List<PlayerEntity>>(IMemoryCacheRepository.PLAYER_KEY)?.FirstOrDefault(x => x.UserId == id);

        if (entity is null)
        {
            _logger.LogError("Failed to find user with id {id}", id);
            throw new NotFoundException($"Failed to find user with id {id}");
        }

        var dto = entity.Adapt<PlayerDTO>();

        return dto;
    }
    public Task<PlayerDTO> GetByUserName(string userName)
    {
        var entity = _cache.Get<List<PlayerEntity>>(IMemoryCacheRepository.PLAYER_KEY)?.FirstOrDefault(x => x.UserName == userName);

        if (entity is null)
        {
            _logger.LogError("Failed to find user with id {userName}", userName);
            throw new NotFoundException($"Failed to find user with id {userName}");
        }

        var dto = entity.Adapt<PlayerDTO>();

        return Task.FromResult(dto);
    }

    public async Task Delete(Guid id)
    {
        await _cache.DeletePlayer(id);
    }
}
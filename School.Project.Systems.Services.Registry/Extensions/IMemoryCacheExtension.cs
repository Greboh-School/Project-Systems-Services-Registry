using Microsoft.Extensions.Caching.Memory;
using School.Project.Systems.Services.Registry.Models.Entities;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Project.Systems.Services.Registry.Extensions;

public static class IMemoryCacheRepository
{
    public static readonly string SERVER_KEY = "SERVERS";
    public static readonly string PLAYER_KEY = "PLAYERS";

    public static async Task<List<ServerEntity>> GetOrCreateServers(this IMemoryCache cache)
    {
        
        var servers = (await cache.GetOrCreateAsync(SERVER_KEY, _ => Task.FromResult(new List<ServerEntity>())))!;
        return servers;
    }

    public static ServerEntity? GetServer(this IMemoryCache cache, Guid id)
    {
        var result = cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers);

        if (!result)
        {
            return null;
        }

        var server = servers!.FirstOrDefault(x => x.Id == id);
        
        if (server.Current == server.Capacity)
        {
            return null;
        }

        return server;
    }

    public static Task DeleteServer(this IMemoryCache cache, Guid id)
    {
        var result = cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers);
        
        if(!result)
        {
            return Task.CompletedTask;
        }

        var entity = servers!.FirstOrDefault(x => x.Id == id);

        if (entity is null)
        {
            return Task.CompletedTask;
        }
        
        servers!.Remove(entity);

        cache.Set(SERVER_KEY, servers);

        cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players);
        
        if (players is null)
        {
            return Task.CompletedTask;
        }
        
        players.RemoveAll(x => x.ServerId == id);
        cache.Set(PLAYER_KEY, players);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Gets the first server that is available.
    /// </summary>
    /// <param name="cache">The memory cache to search</param>
    /// <returns>The first server in the cache</returns>
    /// <remarks>Adheres to server's capacity and current count</remarks>
    public static ServerEntity? GetFirstAvailableServer(this IMemoryCache cache)
    {
        var result = cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers);
        
        if (!result)
        {
            return null;
        }

        var server = servers!.FirstOrDefault(x => x.Capacity != x.Current);

        if (server is null)
        {
            return null;
        }
        
        servers.FirstOrDefault(x => x.Id == server.Id).Current++;

        cache.Set(SERVER_KEY, servers);

        return server;
    }
    
    public static async Task<List<PlayerEntity>> GetOrCreatePlayers(this IMemoryCache cache)
    {
        var players = (await cache.GetOrCreateAsync(PLAYER_KEY, _ => Task.FromResult(new List<PlayerEntity>())))!;
        return players;
    }

    public static Task DeletePlayer(this IMemoryCache cache, Guid id)
    {
        var result = cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players);
        
        if(!result)
        {
            return Task.CompletedTask;
        }

        var entity = players!.FirstOrDefault(x => x.UserId == id);

        if (entity is null)
        {
            return Task.CompletedTask;
        }
        
        players!.Remove(entity);

        var servers = cache.GetOrCreateServers().Result;
        servers.First(x => x.Id == entity.ServerId).Current--;

        cache.Set(SERVER_KEY, servers);
        cache.Set(PLAYER_KEY, players);

        return Task.CompletedTask;
    }
}
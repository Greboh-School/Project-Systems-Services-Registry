using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using School.Project.Systems.Services.Registry.Extensions;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Options;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Project.Systems.Services.Registry.Services;

public interface IBrokerService
{
    public void Create(MessageDTO dto);
    public void CreateServerQueue(Guid userId);
}

public class BrokerService : IBrokerService
{
    private readonly IMemoryCache _cache;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<BrokerService> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public BrokerService(IMemoryCache cache, IOptions<RabbitMQOptions> options, ILogger<BrokerService> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            UserName = _options.User,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    
    public void Create(MessageDTO dto)
    {
        var type = Enum.GetName(dto.Type);

        dto.Sender ??= "ADMIN";
        
        var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(dto);
        switch (dto.Type)
        {
            case MessageType.Private:
            {
                if (!_cache.TryGetPlayerByUsername(dto.Recipient!, out var player))
                {
                    _logger.LogWarning("Sender: {sender} tried to send a message to {recipient} but the recipient does not have a session!", dto.Sender, dto.Recipient);    
                    throw new NotFoundException("Recipient does not have a session!");
                }

                var routingKey = $"server.{player!.ServerId}.user.{player.UserName}";
                
                _channel.BasicPublish(type, routingKey, null, serializedMessage);
                break;
            }
            case MessageType.Public:
            {
                _channel.BasicPublish(type, "", null, serializedMessage);
                break;
            }
            case MessageType.Guild:
            {
                var routingKey = $"guild.{dto.Recipient}";
                
                _channel.BasicPublish(type, routingKey, null, serializedMessage);
                break;
            }
            default:
                throw new BadRequestException("Invalid message type");
        }
    }

    public void CreateServerQueue(Guid serverId)
    {
        var queueName = $"server.{serverId}";
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: queueName, exchange: Enum.GetName(MessageType.Private), routingKey: $"{queueName}.user.#");
        
        _channel.QueueDeclare(queue: $"{queueName}.public", durable: false, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: $"{queueName}.public", exchange: Enum.GetName(MessageType.Public), routingKey: "");
        
        _channel.QueueDeclare(queue: $"{queueName}.guild", durable: false, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: $"{queueName}.guild", exchange: Enum.GetName(MessageType.Guild), routingKey: "guild.#");
    }
    
}
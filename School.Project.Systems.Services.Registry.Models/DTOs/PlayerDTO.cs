namespace School.Project.Systems.Services.Registry.Models.DTOs;

public sealed record PlayerDTO(Guid UserId, string UserName, Guid ServerId, string ServerAddress, ushort ServerPort);
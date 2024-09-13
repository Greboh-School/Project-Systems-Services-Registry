namespace School.Project.Systems.Services.Registry.Models.Entities;

public sealed record PlayerEntity(string UserName, Guid UserId, Guid ServerId, string ServerAddress, ushort ServerPort);
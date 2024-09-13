namespace School.Project.Systems.Services.Registry.Models.Requests;

public sealed record PlayerConnectRequest(string UserName, Guid UserId);
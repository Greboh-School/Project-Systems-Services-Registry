namespace School.Project.Systems.Services.Registry.Models.Requests;

public sealed record ServerRegistrationRequest(string Address, ushort Port, string ListenAddress);
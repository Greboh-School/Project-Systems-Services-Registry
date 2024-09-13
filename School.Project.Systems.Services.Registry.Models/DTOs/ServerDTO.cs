namespace School.Project.Systems.Services.Registry.Models.DTOs;

public sealed record ServerDTO(Guid Id, string Address, ushort Port, int Capacity, int Current);
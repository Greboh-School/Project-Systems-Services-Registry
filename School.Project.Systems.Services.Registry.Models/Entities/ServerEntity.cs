namespace School.Project.Systems.Services.Registry.Models.Entities;

public class ServerEntity
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public ushort Port { get; set; }
    public string ListenAddress { get; set; }
    public int Capacity { get; set; }
    public int Current { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Address: {Address}, Port: {Port.ToString()}, ListenAddress: {ListenAddress}";
    }
}
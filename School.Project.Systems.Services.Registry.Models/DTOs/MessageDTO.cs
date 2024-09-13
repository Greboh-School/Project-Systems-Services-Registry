namespace School.Project.Systems.Services.Registry.Models.DTOs;

public enum MessageType
{
    None = 0,
    Public = 1,
    Private = 2,
    Guild = 3,
}

public class MessageDTO
{
    public MessageType Type { get; set; }
    public string Content { get; set; }
    public string? Sender { get; set; }
    public string? Recipient { get; set; }
}
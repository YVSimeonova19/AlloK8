using System;

namespace AlloK8.DAL.Models;

public class Message
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public UserProfile? Sender { get; set; }

    public UserProfile? Receiver { get; set; }
}
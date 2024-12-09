﻿using System;

namespace AlloK8.DAL.Models;

public class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Type { get; set; }

    public string? Message { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}
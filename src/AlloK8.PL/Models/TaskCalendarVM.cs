using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.PL.Models;

public class TaskCalendarVM
{
    public int Id { get; set; }

    public string? title { get; set; }

    public DateTime start { get; set; }

    public DateTime end { get; set; }
}
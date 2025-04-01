using System;
using System.Collections.Generic;

namespace AlloK8.Common.Models.Report;

public class ReportVM
{
    public string? TaskName { get; set; }
    public string? TaskDescription { get; set; }
    public bool IsPriority { get; set; }
    public string? Progress { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Assignees { get; set; } = [];
    public List<string> Labels { get; set; } = [];
}
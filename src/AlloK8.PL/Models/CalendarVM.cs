using System.Collections.Generic;

namespace AlloK8.PL.Models;

public class CalendarVM
{
    public List<TaskCalendarVM> Tasks = new List<TaskCalendarVM>();

    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}
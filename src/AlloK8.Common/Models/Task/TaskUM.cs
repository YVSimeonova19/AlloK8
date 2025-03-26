using System;
using System.ComponentModel.DataAnnotations;

namespace AlloK8.BLL.Common.Tasks;

public class TaskUM
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public bool? IsPriority { get; set; } = null;
    public int? Position { get; set; } = null;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? StartDate { get; set; } = null;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DueDate { get; set; } = null;
    public int? ColumnId { get; set; } = null;
}
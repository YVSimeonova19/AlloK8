using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models.Requests;

public class CreateTaskRequest
{
    public int ProjectId { get; set; }
    public string? Title { get; set; }
    public int ColumnId { get; set; }
}
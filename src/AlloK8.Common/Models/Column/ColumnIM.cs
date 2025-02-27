using System.ComponentModel.DataAnnotations;

namespace AlloK8.Common.Models.Column;

public class ColumnIM
{
    [Required]
    public string? Name { get; set; }
    
}
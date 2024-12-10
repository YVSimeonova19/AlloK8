namespace AlloK8.PL.Models;

public class ErrorVM
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
namespace AlloK8.PL.Models.Requests;

public class MoveTaskRequest
{
    public int Id { get; set; }
    public int ColumnId { get; set; }
    public int Position { get; set; }
}
using System;

namespace AlloK8.DAL.Models;

public class Column
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public string? Name { get; set; }
    public int Position { get; set; }

    public Board? Board { get; set; }
}
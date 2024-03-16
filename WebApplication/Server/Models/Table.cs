using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("TABLE")]
public class Table
{
    [Key]
    public int TableID { get; set; }

    public int Number { get; set; }

    public int Seats { get; set; }

    public string Status { get; set; }

    public List<Guest> Guests { get; set; } = new List<Guest>();
}

public class TableDTO
{
    public int TableID { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; }
}
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

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Table number most be a positive value")]
    public int Number { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Table seats most be a positive value")]
    public int Seats { get; set; }

    public string Status { get; set; } // = "Available"; // "Available", "Occupied", "Full"

    public List<Guest> Guests { get; set; } = new List<Guest>();
}

public class TableDTO
{
    public int TableID { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; }
}
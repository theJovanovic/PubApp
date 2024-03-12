using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

[Table("GUEST")]
public class Guest
{
    [Key]
    public int GuestID { get; set; }

    public string Name { get; set; }

    public int Money { get; set; }

    public bool HasAllergies { get; set; }

    public bool HasDiscount { get; set; }

    // Foreign Key
    public int TableID { get; set; }

    // Navigation property for Table
    [ForeignKey("TableID")]
    public Table Table { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();
}

public class GuestDTO
{
    public int GuestID { get; set; }
    public string Name { get; set; }
    public int Money { get; set; }
    public bool HasAllergies { get; set; }
    public bool HasDiscount { get; set; }
    public int TableNumber { get; set; }
}

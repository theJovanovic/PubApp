using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

[Table("GUEST")]
public class Guest
{
    [Key]
    public int GuestID { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "Name can't have more than 50 characters")]
    public string Name { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Money can't be negative")]
    public int Money { get; set; }

    public bool HasAllergies { get; set; }

    public bool HasDiscount { get; set; }

    public int TableID { get; set; }

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

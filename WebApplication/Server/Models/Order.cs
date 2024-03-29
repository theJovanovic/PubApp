using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("ORDER")]
public class Order
{
    [Key]
    public int OrderID { get; set; }

    [Required]
    public DateTime OrderTime { get; set; } = DateTime.Now;

    [Required]
    [RegularExpression("(Pending|Preparing|Completed|Delivered)", ErrorMessage = "Invalid status")]
    public string Status { get; set; } = "Pending"; // Pending, Preparing, Completed, Delivered

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive value")]
    public int Quantity { get; set; }

    [Required]
    public int GuestID { get; set; }

    [ForeignKey("GuestID")]
    public Guest Guest { get; set; }

    [Required]
    public int MenuItemID { get; set; }

    [ForeignKey("MenuItemID")]
    public MenuItem MenuItem { get; set; }

    public int? WaiterID { get; set; }

    [ForeignKey("WaiterID")]
    public Waiter? Waiter { get; set; }
}

public class OrderDTO
{
    public int OrderID { get; set; }
    public DateTime OrderTime { get; set; }
    public string Status { get; set; }
    public int GuestID { get; set; }
}

public class OrderCreateDTO
{
    public int GuestID { get; set; }
    public int MenuItemID { get; set; }
    public int Quantity {  get; set; }
}

public class OrderOverviewDTO
{
    public int OrderID { get; set; }
    public string Name { get; set; }
    public DateTime OrderTime { get; set; }
    public string Status { get; set; }
    public int Quantity { get; set; }
    public int TableNumber { get; set; }
}
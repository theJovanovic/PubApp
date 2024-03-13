using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("ORDER")]
public class Order
{
    [Key]
    public int OrderID { get; set; }

    public DateTime OrderTime { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Pending"; // Pending, Preparing, Completed, Delivered

    public int Quantity { get; set; }

    public int GuestID { get; set; }

    [ForeignKey("GuestID")]
    public Guest Guest { get; set; }

    public int MenuItemID { get; set; }

    [ForeignKey("MenuItemID")]
    public MenuItem MenuItem { get; set; }
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
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
    public string Status { get; set; } // Consider using an enum for predefined statuses

    // Foreign Keys
    public int GuestID { get; set; }

    // Navigation properties
    [ForeignKey("GuestID")]
    public Guest Guest { get; set; }

    // Collection of MenuItem through a junction table (OrderDetail)
    public List<OrderDetail> OrderDetails { get; set; }
}

public class OrderDTO
{
    public int OrderID { get; set; }
    public DateTime OrderTime { get; set; }
    public string Status { get; set; }
    public int GuestID { get; set; }
}
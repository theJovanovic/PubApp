using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

[Table("ORDER_DETAILS")]
public class OrderDetail
{
    [Key]
    public int OrderDetailID { get; set; }
    public int Quantity { get; set; } // Added to track quantity of each item
    public int OrderID { get; set; }
    public int MenuItemID { get; set; }

    // Navigation properties
    [ForeignKey("OrderID")]
    public Order Order { get; set; }

    [ForeignKey("MenuItemID")]
    public MenuItem MenuItem { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("MENU_ITEM")]
public class MenuItem
{
    [Key]
    public int MenuItemID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } // Consider using an enum for predefined categories

    // Collection of OrderDetail to represent Many-to-Many relationship with Order
    [JsonIgnore]
    public List<OrderDetail> OrderDetails { get; set; }
}
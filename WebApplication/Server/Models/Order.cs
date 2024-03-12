using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("ORDER")]
public class Order
{
    [Key]
    public int OrderID { get; set; }
    public DateTime OrderTime { get; set; }
    public string Status { get; set; } // Consider using an enum for predefined statuses

    // Foreign Keys
    public int TableID { get; set; }
    public int WaiterID { get; set; }

    // Navigation properties
    [ForeignKey("TableID")]
    public Table Table { get; set; }

    [ForeignKey("WaiterID")]
    public Waiter Waiter { get; set; }

    // Collection of MenuItem through a junction table (OrderDetail)
    [JsonIgnore]
    public List<OrderDetail> OrderDetails { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Server.Models;

[Table("WAITER")]
public class Waiter
{
    [Key]
    public int WaiterID { get; set; }
    public string Name { get; set; }

    // Navigation property for Orders
    public List<Order> Orders { get; set; }
}
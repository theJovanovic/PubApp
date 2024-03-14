using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

[Table("WAITER")]
public class Waiter
{
    [Key]
    public int WaiterID { get; set; }

    public string Name { get; set; }

    public int Tips { get; set; } = 0;
}

public class WaiterDTO
{
    public int WaiterID { get; set; }
    public string Name { get; set; }
    public int Tips { get; set; }
}
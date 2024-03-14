using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace Server.Models;

public enum Category
{
    Chinese,
    French,
    Indian,
    International,
    Italian,
    Japanese,
    Mexican,
}

[Table("MENU_ITEM")]
public class MenuItem
{
    [Key]
    public int MenuItemID { get; set; }

    public string Name { get; set; }

    public int Price { get; set; }

    public string Category { get; set; }

    public bool HasAllergens { get; set; }

    public List<Order> Orders { get; set; }
}

public class MenuItemDTO
{
    public int MenuItemID { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string Category { get; set; }
    public bool HasAllergens { get; set; }
}
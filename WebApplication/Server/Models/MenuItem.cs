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

    [Required]
    [MaxLength(80, ErrorMessage = "Name can't have more than 80 characters")]
    public string Name { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Price can't be negative")]
    public int Price { get; set; }

    [Required]
    [EnumDataType(typeof(Category), ErrorMessage = "Invalid category")]
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
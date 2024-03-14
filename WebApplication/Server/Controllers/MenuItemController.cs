using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuItemController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMapper _mapper;

    public MenuItemController(PubContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/MenuItem
    [HttpGet]
    public async Task<ActionResult> GetMenuItems()
    {
        var menuItems = await _context.MenuItems
            .Select(mi => _mapper.Map<MenuItemDTO>(mi))
            .ToListAsync();

        return Ok(menuItems);
    }

    // GET: api/MenuItem/order/5
    [HttpGet("order/{guestID}")]
    public async Task<ActionResult> GetMenuItemsForOrder(int guestID)
    {
        var guest = await _context.Guests.FindAsync(guestID);

        if (guest == null)
        {
            return NotFound("Guest with given ID doesn't exist");
        }

        var discountFactor = guest.HasDiscount ? 0.85 : 1.0;

        var menuItemsQuery = _context.MenuItems.AsQueryable();

        if (guest.HasAllergies)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => !mi.HasAllergens);
        }

        var menuItems = await menuItemsQuery
            .Select(mi => new MenuItemDTO
            {
                MenuItemID = mi.MenuItemID,
                Name = mi.Name,
                Price = (int)(mi.Price * discountFactor),
                Category = mi.Category,
                HasAllergens = mi.HasAllergens
            })
            .ToListAsync();

        return Ok(menuItems);
    }

    // GET: api/MenuItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetMenuItem(int id)
    {
        var menuItem = await _context.MenuItems
            .Where(mi => mi.MenuItemID == id)
            .Select(mi => _mapper.Map<MenuItemDTO>(mi))
            .FirstOrDefaultAsync();

        if (menuItem == null)
        {
            return NotFound("Item with given ID doesn't exist");
        }

        return Ok(menuItem);
    }

    // GET: api/MenuItem/categories
    [HttpGet("categories")]
    public async Task<ActionResult> GetMenuCategories()
    {
        List<string> categories = Enum.GetNames(typeof(Category)).ToList();
        return Ok(categories);
    }

    // POST: api/MenuItem
    [HttpPost]
    public async Task<ActionResult> PostMenuItem(MenuItemDTO menuItemDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Asserts
        if (menuItemDTO.Name.Length > 80)
        {
            return BadRequest("Name can't have more than 80 character");
        }
        if (menuItemDTO.Price < 0)
        {
            return BadRequest("Price can't be negative");
        }

        // check if menuItemDTO.Category belongs to Category enum defined in MenuItem.cs
        bool categoryExists = Enum.IsDefined(typeof(Category), menuItemDTO.Category);

        if (categoryExists == false)
        {
            return BadRequest("The given category doesn't exist");
        }

        var menuItem = _mapper.Map<MenuItem>(menuItemDTO);

        await _context.MenuItems.AddAsync(menuItem);
        await _context.SaveChangesAsync();

        var result = await _context.MenuItems
            .Where(mi => mi.MenuItemID == menuItem.MenuItemID)
            .Select(mi => _mapper.Map<MenuItemDTO>(mi))
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetMenuItem), new { id = result.MenuItemID }, result);
    }

    // PUT: api/MenuItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTable(int id, MenuItemDTO menuItemDTO)
    {
        // Asserts
        if (menuItemDTO.Name.Length > 80)
        {
            return BadRequest("Name can't have more than 80 character");
        }
        if (menuItemDTO.Price < 0)
        {
            return BadRequest("Price can't be negative");
        }

        if (id != menuItemDTO.MenuItemID)
        {
            return BadRequest("Item IDs don't match");
        }

        // check if menuItemDTO.Category belongs to Category enum defined in MenuItem.cs
        bool categoryExists = Enum.IsDefined(typeof(Category), menuItemDTO.Category);

        if (categoryExists == false)
        {
            return BadRequest("The given category doesn't exist");
        }

        var menuItem = await _context.MenuItems.FindAsync(menuItemDTO.MenuItemID);

        if (menuItem == null)
        {
            return NotFound("Item with given ID doesn't exist");
        }

        menuItem.Name = menuItemDTO.Name;
        menuItem.Price = menuItemDTO.Price;
        menuItem.Category = menuItemDTO.Category;
        menuItem.HasAllergens = menuItemDTO.HasAllergens;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/MenuItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
        {
            return NotFound("Item with given ID doesn't exist");
        }

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

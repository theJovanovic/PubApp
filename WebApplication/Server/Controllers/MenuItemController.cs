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
            return NotFound();
        }

        var menuItemsQuery = _context.MenuItems.AsQueryable();

        if (guest.HasAllergies)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => !mi.HasAllergens);
        }

        var menuItems = await menuItemsQuery
            .Select(mi => _mapper.Map<MenuItemDTO>(mi))
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
            return NotFound();
        }

        return Ok(menuItem);
    }

    // POST: api/MenuItem
    [HttpPost]
    public async Task<ActionResult> PostMenuItem(MenuItemDTO menuItemDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
        if (id != menuItemDTO.MenuItemID)
        {
            return BadRequest();
        }

        var menuItem = await _context.MenuItems.FindAsync(menuItemDTO.MenuItemID);

        if (menuItem == null)
        {
            return NotFound();
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
            return NotFound();
        }

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

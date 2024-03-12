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

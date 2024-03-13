using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GuestController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMapper _mapper;

    public GuestController(PubContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Guest
    [HttpGet]
    public async Task<ActionResult> GetGuests()
    {
        var guests = await _context.Guests
            .Select(g => new GuestDTO
            {
                GuestID = g.GuestID,
                Name = g.Name,
                Money = g.Money,
                HasAllergies = g.HasAllergies,
                HasDiscount = g.HasDiscount,
                TableNumber = g.Table.Number
            })
            .ToListAsync();

        return Ok(guests);
    }

    // GET: api/Guest/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetGuest(int id)
    {
        var guest = await _context.Guests
            .Where(g => g.GuestID == id)
            .Select(g => new GuestDTO
            {
                GuestID = g.GuestID,
                Name = g.Name,
                Money = g.Money,
                HasAllergies = g.HasAllergies,
                HasDiscount = g.HasDiscount,
                TableNumber = g.Table.Number
            })
            .FirstOrDefaultAsync();

        if (guest == null)
        {
            return NotFound();
        }

        return Ok(guest);
    }

    // GET: api/Guest/info/5
    [HttpGet("info/{id}")]
    public async Task<ActionResult> GetGuestInfo(int id)
    {
        var guest = await _context.Guests
            .Where(g => g.GuestID == id)
            .FirstOrDefaultAsync();

        if (guest == null)
        {
            return NotFound();
        }

        var guestInfo = new
        {
            GuestID = guest.GuestID,
            Name = guest.Name,
            Money = guest.Money,
            HasAllergies = guest.HasAllergies,
            HasDiscount = guest.HasDiscount,
            TableID = guest.TableID,
            TableNumber = await _context.Tables
            .Where(t => t.TableID == guest.TableID)
            .Select(t => t.Number)
            .FirstOrDefaultAsync(),
            Orders = await _context.Orders
            .Where(o => o.GuestID == guest.GuestID)
            .Where(o => o.Status != "Delivered")
            .Select(o => new
            {
                OrderID = o.OrderID,
                Name = o.MenuItem.Name,
                Price = o.MenuItem.Price,
                Status = o.Status
            })
            .ToListAsync()
        };

        return Ok(guestInfo);
    }

    // POST: api/Guest
    [HttpPost]
    public async Task<ActionResult> PostGuest(GuestDTO guestDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.Number == guestDTO.TableNumber);
        if (existingTable == null)
        {
            return NotFound();
        }

        var guest = new Guest
        {
            GuestID = guestDTO.GuestID,
            Name = guestDTO.Name,
            Money = guestDTO.Money,
            HasAllergies = guestDTO.HasAllergies,
            HasDiscount = guestDTO.HasDiscount,
            TableID = existingTable.TableID
        };
        
        await _context.Guests.AddAsync(guest);
        await _context.SaveChangesAsync();

        var result = await _context.Guests
            .Where(g => g.GuestID == guest.GuestID)
            .Select(g => new GuestDTO
            {
                GuestID = g.GuestID,
                Name = g.Name,
                Money = g.Money,
                HasAllergies = g.HasAllergies,
                HasDiscount = g.HasDiscount,
                TableNumber = g.Table.Number
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetGuest), new { id = result.GuestID }, result);
    }

    // PUT: api/Guest/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGuest(int id, GuestDTO guestDTO)
    {
        if (id != guestDTO.GuestID)
        {
            return BadRequest();
        }

        var guest = await _context.Guests.FindAsync(guestDTO.GuestID);

        if (guest == null)
        {
            return NotFound();
        }

        var guestsTable = await _context.Tables.FirstOrDefaultAsync(t => t.TableID == guest.TableID);

        //change other properties
        guest.Name = guestDTO.Name;
        guest.Money = guestDTO.Money;
        guest.HasAllergies = guestDTO.HasAllergies;
        guest.HasDiscount = guestDTO.HasDiscount;

        // change table
        if (guestDTO.TableNumber != guestsTable.Number)
        {
            var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.Number == guestDTO.TableNumber);
            if (existingTable == null)
            {
                return NotFound();
            }
            guest.TableID = existingTable.TableID;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Guest/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null)
        {
            return NotFound();
        }

        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

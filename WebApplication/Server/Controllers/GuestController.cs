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
            return NotFound("Guest with given ID doesn't exist");
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
            return NotFound("Guest with given ID doesn't exist");
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
            .Select(o => new
            {
                OrderID = o.OrderID,
                Name = o.MenuItem.Name,
                Price = guest.HasDiscount ? (int)(o.MenuItem.Price * 0.85) : o.MenuItem.Price,
                Status = o.Status,
                Quantity = o.Quantity,
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

        // Asserts
        if (guestDTO.Name.Length > 50)
        {
            return BadRequest("Name can't have more than 50 characters");
        }
        if (guestDTO.Money < 0)
        {
            return BadRequest("Money can't be negative");
        }
        if (guestDTO.TableNumber < 1)
        {
            return BadRequest("Table number must be positive value");
        }

        var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.Number == guestDTO.TableNumber);

        if (existingTable == null)
        {
            return NotFound("Table with given number doesn't exist");
        }

        var numOfPeopleAtTable = _context.Guests
            .Where(g => g.TableID == existingTable.TableID)
            .Count();

        if (numOfPeopleAtTable < existingTable.Seats - 1)
        {
            existingTable.Status = "Occupied";
        }
        else if (numOfPeopleAtTable == existingTable.Seats - 1)
        {
            existingTable.Status = "Full";
        }
        else if (numOfPeopleAtTable == existingTable.Seats)
        {
            return Conflict("Table is already full");
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
        // Asserts
        if (guestDTO.Name.Length > 50)
        {
            return BadRequest("Name can't have more than 50 characters");
        }
        if (guestDTO.Money < 0)
        {
            return BadRequest("Money can't be negative");
        }
        if (guestDTO.TableNumber < 1)
        {
            return BadRequest("Table number must be positive value");
        }

        if (id != guestDTO.GuestID)
        {
            return BadRequest("Guest IDs don't match");
        }

        var guest = await _context.Guests.FindAsync(guestDTO.GuestID);

        if (guest == null)
        {
            return NotFound("Guest with given ID doesn't exist");
        }

        var oldTable = await _context.Tables.FirstOrDefaultAsync(t => t.TableID == guest.TableID);

        //change other properties
        guest.Name = guestDTO.Name;
        guest.Money = guestDTO.Money;
        guest.HasAllergies = guestDTO.HasAllergies;
        guest.HasDiscount = guestDTO.HasDiscount;

        // change table
        if (guestDTO.TableNumber != oldTable.Number)
        {
            var newTable = await _context.Tables.FirstOrDefaultAsync(t => t.Number == guestDTO.TableNumber);
            if (newTable == null)
            {
                return NotFound("Table with given table number doesn't exist");
            }

            //change new table status
            var newTableGuestCount = _context.Guests
                .Where(g => g.TableID == newTable.TableID)
                .Count();
            if (newTableGuestCount + 1 > newTable.Seats)
            {
                return BadRequest("Table is already full");
            }
            else if (newTableGuestCount + 1 == newTable.Seats)
            {
                newTable.Status = "Full";
            }
            else if (newTableGuestCount + 1 < newTable.Seats)
            {
                newTable.Status = "Occupied";
            }

            //change old table status
            var oldTableGuestCount = _context.Guests
                    .Where(g => g.TableID == oldTable.TableID)
                    .Count();
            if (oldTableGuestCount - 1 < 0)
            {
                return BadRequest("The guest is not registed on table");
            }
            else if (oldTableGuestCount - 1 == 0)
            {
                oldTable.Status = "Available";
            }
            else
            {
                oldTable.Status = "Occupied";
            }

            guest.TableID = newTable.TableID;
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
            return NotFound("Guest with given ID doesn't exist");
        }

        //update table
        var table = await _context.Tables.FindAsync(guest.TableID);
        if (table == null)
        {
            return NotFound("Table with given ID doesn't exist");
        }

        var guestsAtTable = _context.Guests
            .Where(g => g.TableID == table.TableID)
            .Count();
        if (guestsAtTable - 1 < 0)
        {
            return BadRequest("The guest is not registed on table");
        }
        else if (guestsAtTable - 1 == 0)
        {
            table.Status = "Available";
        }
        else
        {
            table.Status = "Occupied";
        }

        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

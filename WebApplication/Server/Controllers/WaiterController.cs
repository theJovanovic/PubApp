using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaiterController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMapper _mapper;

    public WaiterController(PubContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Waiter
    [HttpGet]
    public async Task<ActionResult> GetWaiters()
    {
        var waiters = await _context.Waiters
            .Select(w => _mapper.Map<WaiterDTO>(w))
            .ToListAsync();

        return Ok(waiters);
    }

    // GET: api/Waiter/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetWaiter(int id)
    {
        var waiter = await _context.Waiters.FindAsync(id);

        if (waiter == null)
        {
            return NotFound("Waiter with given ID doesn't exist");
        }
        
        var result = _mapper.Map<WaiterDTO>(waiter);

        return Ok(result);
    }

    // POST: api/Waiter
    [HttpPost]
    public async Task<ActionResult> PostWaiter(WaiterDTO waiterDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //Asserts
        if (waiterDTO.Name.Length > 50)
        {
            return BadRequest("Name can't have more than 50 characters");
        }
        if (waiterDTO.Tips != 0)
        {
            return BadRequest("Tips can't be set");
        }

        var waiter = new Waiter
        {
            Name = waiterDTO.Name,
            Tips = waiterDTO.Tips
        };

        await _context.Waiters.AddAsync(waiter);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<WaiterDTO>(waiter);

        return CreatedAtAction(nameof(GetWaiter), new { id = result.WaiterID }, result);
    }

    // PUT: api/Waiter/5
    [HttpPut("{id}")]
    public async Task<ActionResult> PutWaiter(int id, WaiterDTO waiterDTO)
    {
        //Asserts
        if (waiterDTO.Name.Length > 50)
        {
            return BadRequest("Name can't have more than 50 characters");
        }
        if (waiterDTO.Tips < 0)
        {
            return BadRequest("Tips can't be negative");
        }

        if (id != waiterDTO.WaiterID)
        {
            return BadRequest("Waiter IDs don't match");
        }

        var waiter = await _context.Waiters.FindAsync(waiterDTO.WaiterID);

        if (waiter == null)
        {
            return NotFound("Waiter with given ID doesn't exist");
        }

        //change other properties
        waiter.Name = waiterDTO.Name;
        waiter.Tips = waiterDTO.Tips;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Waiter/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWaiter(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waiter = await _context.Waiters.FindAsync(id);

        if (waiter == null)
        {
            return NotFound("Waiter with given ID doesn't exist");
        }

        _context.Waiters.Remove(waiter);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
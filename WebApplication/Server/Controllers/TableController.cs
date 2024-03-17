using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMapper _mapper;

    public TableController(PubContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Table
    [HttpGet]
    public async Task<ActionResult> GetTables()
    {
        var tables = await _context.Tables
            .Select(t => _mapper.Map<TableDTO>(t))
            .ToListAsync();

        return Ok(tables);
    }

    // GET: api/Table/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTable(int id)
    {
        var table = await _context.Tables
            .Where(table => table.TableID == id)
            .Select(t => _mapper.Map<TableDTO>(t    ))
            .FirstOrDefaultAsync();

        if (table == null)
        {
            return NotFound("Table with given ID doesn't exist");
        }

        return Ok(table);
    }

    // GET: api/Table/info/5
    [HttpGet("info/{id}")]
    public async Task<ActionResult> GetTableInfo(int id)
    {
        var table = await _context.Tables
            .Where(table => table.TableID == id)
            .FirstOrDefaultAsync();

        if (table == null)
        {
            return NotFound("Table with given ID doesn't exist");
        }

        var tableInfo = new
        {
            TableID = table.TableID,
            Number = table.Number,
            Seats = table.Seats,
            Status = table.Status,
            Guests = await _context.Guests
            .Where(g => g.TableID == id)
            .Select(g => new
            {
                GuestID = g.GuestID,
                Name = g.Name
            }).ToListAsync()
        };

        return Ok(tableInfo);
    }

    // POST: api/Table
    [HttpPost]
    public async Task<ActionResult> PostTable(TableDTO tableDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //Asserts
        if (tableDTO.Number < 1)
        {
            return BadRequest("Table number must be a positive value");
        }
        if (tableDTO.Seats < 1)
        {
            return BadRequest("Seats must be a positive value");
        }

        var existingTable = await _context.Tables
            .Where(t => t.Number ==  tableDTO.Number)
            .FirstOrDefaultAsync();

        if (existingTable != null)
        {
            return Conflict("Table with the same number already exist");
        }

        var table = _mapper.Map<Table>(tableDTO);

        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();

        var result = await _context.Tables
            .Where(t => t.TableID == table.TableID)
            .Select(t => _mapper.Map<TableDTO>(t))
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetTable), new { id = result.TableID }, result);
    }

    // PUT: api/Table/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTable(int id, TableDTO tableDTO)
    {
        //Asserts
        if (tableDTO.Number < 1)
        {
            return BadRequest("Table number must be a positive value");
        }
        if (tableDTO.Seats < 1)
        {
            return BadRequest("Seats must be a positive value");
        }

        if (id != tableDTO.TableID)
        {
            return BadRequest("Table IDs doesn't match");
        }

        var table = await _context.Tables.FindAsync(tableDTO.TableID);

        // check if table we want to edit exists
        if (table == null)
        {
            return NotFound("Table with given ID doesn't exist");
        }

        var tableWithSameNumber = await _context.Tables
            .Where(t => t.Number == tableDTO.Number)
            .FirstOrDefaultAsync();

        // check if table exists with the number we want to edit
        if (tableWithSameNumber != null && tableWithSameNumber.TableID != table.TableID)
        {
            return Conflict("Table with the same number already exists");
        }

        table.Number = tableDTO.Number;
        table.Seats = tableDTO.Seats;
        table.Status = tableDTO.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Table/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var table = await _context.Tables.FindAsync(id);

        if (table == null)
        {
            return NotFound("Table with given ID doesn't exist");
        }

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

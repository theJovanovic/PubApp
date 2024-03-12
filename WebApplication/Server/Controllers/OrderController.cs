using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMapper _mapper;

    public OrderController(PubContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Order
    [HttpGet]
    public async Task<ActionResult> GetOrders()
    {
        var guests = await _context.Orders
            .Select(o => _mapper.Map<OrderDTO>(o))
            .ToListAsync();

        return Ok(guests);
    }

    // GET: api/Order/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrder(int id)
    {
        var order = await _context.Orders
            .Where(o => o.OrderID == id)
            .Select(o => _mapper.Map<OrderDTO>(o))
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // POST: api/Order
    [HttpPost]
    public async Task<ActionResult> PostOrder(OrderDTO orderDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var order = _mapper.Map<Order>(orderDTO);
        
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var result = await _context.Orders
            .Where(o => o.OrderID == order.OrderID)
            .Select(g => _mapper.Map<OrderDTO>(order))
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = result.OrderID }, result);
    }

}

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
        var orders = await _context.Orders
            .Select(o => _mapper.Map<OrderDTO>(o))
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/Order/overview
    [HttpGet("overview")]
    public async Task<ActionResult> GetOrdersOverview()
    {
        var random = new Random();
        var ordersToUpdate = _context.Orders
            .Where(o => o.Status != "Delivered")
            .Where(o => o.Status != "Completed")
            .ToList();

        foreach (var order in ordersToUpdate)
        {
            // 15% chance
            if (random.Next(100) < 15)
            {
                order.Status = ChangeOrderStatus(order.Status);
            }
        }

        await _context.SaveChangesAsync();

        var orders = await _context.Orders
            .Where(o => o.Status != "Delivered")
            .Select(o => new OrderOverviewDTO
            {
                OrderID = o.OrderID,
                Name = o.MenuItem.Name,
                OrderTime = o.OrderTime,
                Status = o.Status,
                Quantity = o.Quantity,
                TableNumber = o.Guest.Table.Number
            })
            .ToListAsync();

        return Ok(orders);
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
            return NotFound("Order with given ID doesn't exist");
        }

        return Ok(order);
    }

    // POST: api/Order
    [HttpPost]
    public async Task<ActionResult> PostOrder(OrderCreateDTO orderDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //Asserts
        if (orderDTO.Quantity < 1)
        {
            return BadRequest("Quantity must be a positive value");
        }

        var order = new Order
        {
            GuestID = orderDTO.GuestID,
            Quantity = orderDTO.Quantity,
            MenuItemID = orderDTO.MenuItemID,
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<OrderDTO>(order);

        return CreatedAtAction(nameof(GetOrder), new { id = result.OrderID }, result);
    }

    // PUT: api/Order/5/Waiter/1
    [HttpPut("{orderID}/Waiter/{waiterID}")]
    public async Task<ActionResult> DeliverOrder(int orderID, int waiterID)
    {
        var order = await _context.Orders.FindAsync(orderID);
        var waiter = await _context.Waiters.FindAsync(waiterID);

        if (order == null)
        {
            return NotFound("Order with given ID doesn't exist");
        }

        if (waiter == null)
        {
            return NotFound("Waiter with given ID doesn't exist");
        }

        if (order.Status != "Completed")
        {
            return BadRequest("Order is not completed");
        }

        order.Status = "Delivered";
        order.WaiterID = waiterID;
    
        await _context.SaveChangesAsync();
        
        return NoContent();
    }


    // DELETE: api/Order/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound("Order with given ID doesn't exist");
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Order/pay/5/100
    [HttpDelete("pay/{id}/{tip}")]
    public async Task<IActionResult> PayOrder(int id, int tip)
    {
        //Asserts
        if (tip < 0)
        {
            return BadRequest("Tip can't be a negative value");
        }

        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound("Order with given ID doesn't exist");
        }

        // remove money from guest
        var guest = await _context.Guests.FindAsync(order.GuestID);
        if (guest == null)
        {
            return NotFound("Guest with given ID doesn't exist");
        }

        var menuItem = await _context.MenuItems.FindAsync(order.MenuItemID);
        if (menuItem == null)
        {
            return NotFound("Item with given ID doesn't exist");
        }

        int discountedPrice = menuItem.Price;

        if(guest.HasDiscount)
        {
            discountedPrice = (int)(menuItem.Price * 0.85); // 15% discount
        }

        int totalOrderCost = discountedPrice * order.Quantity + tip;

        // check if guest has enough money to pay the order
        if (guest.Money < totalOrderCost)
        {
            return BadRequest("Guest doesn't have enough money to pay the order");
        }

        guest.Money -= totalOrderCost;

        // add tip to waiter
        var waiter = await _context.Waiters.FindAsync(order.WaiterID);
        if (waiter == null)
        {
            return NotFound("Waiter with given ID doesn't exist");
        }
        waiter.Tips += tip;
        await _context.SaveChangesAsync();

        // delete order
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string ChangeOrderStatus(string status)
    {
        string newStatus = status;
        switch (status)
        {
            case "Pending":
                newStatus = "Preparing";
                break;
            case "Preparing":
                newStatus = "Completed";
                break;
            default:
                break;
        }
        return newStatus;
    }
}

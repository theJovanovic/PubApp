using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace PlaywrightTests;

public class DatabaseRefresher
{
    public static async Task AddDataAsync(PubContext _context)
    {
        // Remove everything
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [TABLE]");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pub.dbo.TABLE', RESEED, 0)");

        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [GUEST]");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pub.dbo.GUEST', RESEED, 0)");

        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [MENU_ITEM]");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pub.dbo.MENU_ITEM', RESEED, 0)");

        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [WAITER]");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pub.dbo.WAITER', RESEED, 0)");

        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [ORDER]");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pub.dbo.ORDER', RESEED, 0)");

        // AddAsync Tables
        await _context.Tables.AddAsync(new Table { Number = 101, Seats = 5, Status = "Available" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 102, Seats = 2, Status = "Full" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 103, Seats = 3, Status = "Occupied" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 104, Seats = 7, Status = "Occupied" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 105, Seats = 10, Status = "Available" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 106, Seats = 3, Status = "Available" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 107, Seats = 3, Status = "Full" });
        await _context.SaveChangesAsync();

        await _context.Tables.AddAsync(new Table { Number = 108, Seats = 5, Status = "Occupied" });
        await _context.SaveChangesAsync();


        // AddAsync Guests
        await _context.Guests.AddAsync(new Guest { Name = "Dusan", Money = 1000, HasAllergies = false, HasDiscount = false, TableID = 2 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Stefan", Money = 800, HasAllergies = false, HasDiscount = true, TableID = 2 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Ana", Money = 1370, HasAllergies = false, HasDiscount = true, TableID = 3 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Boris", Money = 960, HasAllergies = true, HasDiscount = false, TableID = 3 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Ena", Money = 2800, HasAllergies = true, HasDiscount = false, TableID = 4 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Marko", Money = 500, HasAllergies = true, HasDiscount = true, TableID = 4 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Nikola", Money = 1264, HasAllergies = true, HasDiscount = false, TableID = 4 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Jovana", Money = 1110, HasAllergies = false, HasDiscount = false, TableID = 7 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Ivana", Money = 735, HasAllergies = true, HasDiscount = true, TableID = 7 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Ilija", Money = 1234, HasAllergies = true, HasDiscount = false, TableID = 7 });
        await _context.SaveChangesAsync();

        await _context.Guests.AddAsync(new Guest { Name = "Pavle", Money = 999, HasAllergies = false, HasDiscount = true, TableID = 8 });
        await _context.SaveChangesAsync();

        // AddAsync Menu Items
        await _context.MenuItems.AddAsync(new MenuItem { Name = "Burger", Price = 350, Category = "International", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Dumplings", Price = 100, Category = "Chinese", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Snails", Price = 460, Category = "French", HasAllergens = true });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Chicken Curry", Price = 300, Category = "Indian", HasAllergens = true });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Pasta Carbonara", Price = 450, Category = "Italian", HasAllergens = true });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Fish and Chips", Price = 290, Category = "International", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Pizza Margherita", Price = 500, Category = "Italian", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Sushi", Price = 800, Category = "Japanese", HasAllergens = true });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Ramen", Price = 450, Category = "Japanese", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Tacos", Price = 390, Category = "Mexican", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Cheese Cake", Price = 300, Category = "International", HasAllergens = false });
        await _context.SaveChangesAsync();

        await _context.MenuItems.AddAsync(new MenuItem { Name = "Ice Cream", Price = 150, Category = "International", HasAllergens = false });
        await _context.SaveChangesAsync();

        // AddAsync Waiters
        await _context.Waiters.AddAsync(new Waiter { Name = "David", Tips = 0 });
        await _context.SaveChangesAsync();

        await _context.Waiters.AddAsync(new Waiter { Name = "Dimitrije", Tips = 100 });
        await _context.SaveChangesAsync();

        await _context.Waiters.AddAsync(new Waiter { Name = "Sara", Tips = 600 });
        await _context.SaveChangesAsync();

        await _context.Waiters.AddAsync(new Waiter { Name = "Kristina", Tips = 440 });
        await _context.SaveChangesAsync();

        await _context.Waiters.AddAsync(new Waiter { Name = "Andjelija", Tips = 120 });
        await _context.SaveChangesAsync();

        await _context.Waiters.AddAsync(new Waiter { Name = "Mihajlo", Tips = 0 });
        await _context.SaveChangesAsync();

        // AddAsync Orders
        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:30:00"), Status = "Preparing", Quantity = 3, GuestID = 1, MenuItemID = 10, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:33:20"), Status = "Pending", Quantity = 1, GuestID = 2, MenuItemID = 8, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:31:18"), Status = "Preparing", Quantity = 2, GuestID = 2, MenuItemID = 6, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:37:02"), Status = "Pending", Quantity = 5, GuestID = 3, MenuItemID = 4, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:38:48"), Status = "Completed", Quantity = 1, GuestID = 4, MenuItemID = 2, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:40:33"), Status = "Completed", Quantity = 2, GuestID = 4, MenuItemID = 11, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:47:30"), Status = "Completed", Quantity = 1, GuestID = 4, MenuItemID = 1, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:49:42"), Status = "Delivered", Quantity = 1, GuestID = 5, MenuItemID = 10, WaiterID = 3 });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:50:12"), Status = "Pending", Quantity = 3, GuestID = 1, MenuItemID = 5, WaiterID = null });
        await _context.SaveChangesAsync();

        await _context.Orders.AddAsync(new Order { OrderTime = DateTime.Parse("2024-03-19T12:58:55"), Status = "Delivered", Quantity = 3, GuestID = 7, MenuItemID = 7, WaiterID = 1 });
        await _context.SaveChangesAsync();
    }
}

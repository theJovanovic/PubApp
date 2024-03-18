using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_GetOrder_Tests
    {
        private static PubContext _context;
        private static OrderController _controller;
        private static IMapper _mapper;
        private IDbContextTransaction _transaction;

        [SetUp]
        public void SetUp()
        {
            // Setup InMemory database
            var options = new DbContextOptionsBuilder<PubContext>()
                .UseInMemoryDatabase(databaseName: "PubTestDb")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new PubContext(options);

            // Setup AutoMapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _controller = new OrderController(_context, _mapper);

            // Seed the database
            //Waiters
            _context.Waiters.Add(new Waiter { WaiterID = 1, Name = "Waiter 1", Tips = 100 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 2, Name = "Waiter 2", Tips = 200 });
            _context.SaveChanges();

            ////MenuItems
            //_context.MenuItems.Add(new MenuItem
            //{
            //    MenuItemID = 1,
            //    Name = "Item 1",
            //    Category = "Category 1",
            //    HasAllergens = false,
            //    Price = 100
            //});
            //_context.SaveChanges();
            //_context.MenuItems.Add(new MenuItem
            //{
            //    MenuItemID = 2,
            //    Name = "Item 2",
            //    Category = "Category 2",
            //    HasAllergens = true,
            //    Price = 200
            //});
            //_context.SaveChanges();
            //_context.MenuItems.Add(new MenuItem
            //{
            //    MenuItemID = 3,
            //    Name = "Item 3",
            //    Category = "Category 3",
            //    HasAllergens = true,
            //    Price = 300
            //});
            //_context.SaveChanges();
            //_context.MenuItems.Add(new MenuItem
            //{
            //    MenuItemID = 4,
            //    Name = "Item 4",
            //    Category = "Category 4",
            //    HasAllergens = true,
            //    Price = 400
            //});
            //_context.SaveChanges();

            ////Tables
            //_context.Tables.Add(new Table { TableID = 1, Number = 100, Seats = 2, Status = "Full" });
            //_context.SaveChanges();

            ////Guests
            //_context.Guests.Add(new Guest
            //{
            //    GuestID = 1,
            //    HasAllergies = false,
            //    HasDiscount = true,
            //    Money = 4200,
            //    Name = "Guest 1",
            //    TableID = 1
            //});
            //_context.SaveChanges();
            //_context.Guests.Add(new Guest
            //{
            //    GuestID = 2,
            //    HasAllergies = true,
            //    HasDiscount = false,
            //    Money = 5200,
            //    Name = "Guest 2",
            //    TableID = 1
            //});

            //Orders
            _context.Orders.Add(new Order
            {
                OrderID = 1,
                OrderTime = DateTime.Now,
                Status = "Pending",
                Quantity = 2,
                GuestID = 1,
                MenuItemID = 1,
                WaiterID = 1
            });
            _context.SaveChanges();
            _context.Orders.Add(new Order
            {
                OrderID = 2,
                OrderTime = new DateTime(2024, 3, 18, 12, 30, 00),
                Status = "Preparing",
                Quantity = 3,
                GuestID = 1,
                MenuItemID = 2,
                WaiterID = 1
            });
            _context.SaveChanges();
            _context.Orders.Add(new Order
            {
                OrderID = 3,
                OrderTime = new DateTime(2024, 3, 18, 12, 31, 00),
                Status = "Completed",
                Quantity = 1,
                GuestID = 2,
                MenuItemID = 3,
                WaiterID = 2
            });
            _context.SaveChanges();
            _context.Orders.Add(new Order
            {
                OrderID = 4,
                OrderTime = new DateTime(2024, 3, 18, 12, 35, 00),
                Status = "Delivered",
                Quantity = 10,
                GuestID = 2,
                MenuItemID = 4,
                WaiterID = 2
            });
            _context.SaveChanges();

            // Setup the transaction just in case
            _transaction = _context.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

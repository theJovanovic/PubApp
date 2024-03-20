using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_PayOrder_Tests
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

            //MenuItems
            _context.MenuItems.Add(new MenuItem
            {
                MenuItemID = 1,
                Name = "Item 1",
                Category = "Category 1",
                HasAllergens = false,
                Price = 100
            });
            _context.SaveChanges();
            _context.MenuItems.Add(new MenuItem
            {
                MenuItemID = 2,
                Name = "Item 2",
                Category = "Category 2",
                HasAllergens = true,
                Price = 200
            });
            _context.SaveChanges();
            _context.MenuItems.Add(new MenuItem
            {
                MenuItemID = 3,
                Name = "Item 3",
                Category = "Category 3",
                HasAllergens = true,
                Price = 300
            });
            _context.SaveChanges();
            _context.MenuItems.Add(new MenuItem
            {
                MenuItemID = 4,
                Name = "Item 4",
                Category = "Category 4",
                HasAllergens = true,
                Price = 400
            });
            _context.SaveChanges();

            //Guests
            _context.Guests.Add(new Guest
            {
                GuestID = 1,
                HasAllergies = false,
                HasDiscount = true,
                Money = 4200,
                Name = "Guest 1",
                TableID = 1
            });
            _context.SaveChanges();
            _context.Guests.Add(new Guest
            {
                GuestID = 2,
                HasAllergies = true,
                HasDiscount = false,
                Money = 5200,
                Name = "Guest 2",
                TableID = 1
            });

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

        [Test]
        public async Task PayOrder_WithValidData_ReturnsNoContent()
        {
            //Arrange
            var existingOrderId = 1;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PayOrder_WithValidData_DeletesOrder()
        {
            //Arrange
            var existingOrderId = 1;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == existingOrderId);
            Assert.That(order, Is.Null);
        }

        [Test]
        public async Task PayOrder_WithValidData_UpdatesWaiterTips()
        {
            //Arrange
            var existingOrderId = 1;
            var existingWaiterId = 1;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            var waiter = await _context.Waiters.FirstOrDefaultAsync(w => w.WaiterID == existingWaiterId);

            Assert.That(waiter, Is.Not.Null);

            Assert.That(waiter.Tips, Is.EqualTo(600));
        }

        [Test]
        public async Task PayOrder_WithValidData_ZeroWaiterTip()
        {
            //Arrange
            var existingOrderId = 1;
            var existingWaiterId = 1;
            var tip = 0;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            var waiter = await _context.Waiters.FirstOrDefaultAsync(w => w.WaiterID == existingWaiterId);

            Assert.That(waiter, Is.Not.Null);

            Assert.That(waiter.Tips, Is.EqualTo(100));
        }

        [Test]
        public async Task PayOrder_WithValidData_UpdatesGuestMoneyHasDiscount()
        {
            //Arrange
            var existingOrderId = 1;
            var existingGuestId = 1;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestID == existingGuestId);

            Assert.That(guest, Is.Not.Null);

            Assert.That(guest.Money, Is.EqualTo(3530));
        }

        [Test]
        public async Task PayOrder_WithValidData_UpdatesGuestMoneyNoDiscount()
        {
            //Arrange
            var existingOrderId = 3;
            var existingGuestId = 2;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestID == existingGuestId);

            Assert.That(guest, Is.Not.Null);

            Assert.That(guest.Money, Is.EqualTo(4400));
        }

        [Test]
        public async Task PayOrder_NegativeTip_ReturnsBadRequest()
        {
            //Arrange
            var existingOrderId = 1;
            var tip = -500;

            //Act
            var result = await _controller.PayOrder(existingOrderId, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PayOrder_WithNonExistingOrderId_ReturnsNotFound()
        {
            //Arrange
            var nonExistingOrderId = 999;
            var tip = 500;

            //Act
            var result = await _controller.PayOrder(nonExistingOrderId, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Order with given ID doesn't exist"));
        }

        [Test]
        public async Task PayOrder_WithNonExistingGuestId_ReturnsNotFound()
        {
            //Arrange
            var order = new Order
            {
                OrderID = 10,
                OrderTime = new DateTime(2024, 3, 18, 14, 15, 16),
                Status = "Preparing",
                Quantity = 5,
                GuestID = 999, //nonExistingGuest
                MenuItemID = 2,
                WaiterID = 1
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var tip = 500;

            //Act
            var result = await _controller.PayOrder(10, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
        }

        [Test]
        public async Task PayOrder_WithNonExistingItemId_ReturnsNotFound()
        {
            //Arrange
            var order = new Order
            {
                OrderID = 10,
                OrderTime = new DateTime(2024, 3, 18, 14, 15, 16),
                Status = "Preparing",
                Quantity = 5,
                GuestID = 1,
                MenuItemID = 999, //nonExistingItemId
                WaiterID = 1 
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var tip = 500;

            //Act
            var result = await _controller.PayOrder(10, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Item with given ID doesn't exist"));
        }

        [Test]
        public async Task PayOrder_WithNonExistingWaiterId_ReturnsNotFound()
        {
            //Arrange
            var order = new Order
            {
                OrderID = 10,
                OrderTime = new DateTime(2024, 3, 18, 14, 15, 16),
                Status = "Preparing",
                Quantity = 5,
                GuestID = 1,
                MenuItemID = 2,
                WaiterID = 999 //nonExistingWaiterId
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var tip = 500;

            //Act
            var result = await _controller.PayOrder(10, tip);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Waiter with given ID doesn't exist"));
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

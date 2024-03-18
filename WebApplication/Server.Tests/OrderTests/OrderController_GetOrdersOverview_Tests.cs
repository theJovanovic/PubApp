using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_GetOrdersOverview_Tests
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
            _context.Orders.Add(new Order
            {
                OrderID = 5,
                OrderTime = new DateTime(2024, 3, 18, 12, 40, 00),
                Status = "Preparing",
                Quantity = 3,
                GuestID = 3,
                MenuItemID = 5,
                WaiterID = 2
            });
            _context.SaveChanges();
            _context.Orders.Add(new Order
            {
                OrderID = 6,
                OrderTime = new DateTime(2024, 3, 18, 12, 45, 00),
                Status = "Preparing",
                Quantity = 8,
                GuestID = 4,
                MenuItemID = 2,
                WaiterID = 2
            });
            _context.SaveChanges();
            _context.Orders.Add(new Order
            {
                OrderID = 7,
                OrderTime = new DateTime(2024, 3, 18, 12, 48, 00),
                Status = "Pending",
                Quantity = 2,
                GuestID = 4,
                MenuItemID = 4,
                WaiterID = 1
            });
            _context.SaveChanges();

            // Setup the transaction just in case
            _transaction = _context.Database.BeginTransaction();
        }

        [Test]
        public async Task GetOrdersOverview_ReturnsCorrectNumberOfNonDeliveredOrders()
        {
            //Arrange
            var expectedNumberOfOrders = 6;

            //Act
            var result = await _controller.GetOrdersOverview();

            //Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var orders = okResult.Value as List<OrderOverviewDTO>;
            Assert.That(orders, Is.Not.Null);

            Assert.That(orders, Has.Count.EqualTo(expectedNumberOfOrders));
        }

        [Test]
        public async Task GetOrdersOverview_ReturnsCorrectId([Values(0, 1, 2, 3)] int index)
        {
            //Act
            var result = await _controller.GetOrdersOverview();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<dynamic>;

            Assert.That(orders, Is.Not.Null);

            Assert.That(orders[index], Has.Property("OrderID").EqualTo(index + 1));
        }

        [Test]
        public async Task GetOrdersOverview_ReturnsEmptyList()
        {
            // Arrange
            _context.MenuItems.RemoveRange(_context.MenuItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetOrdersOverview();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);

            Assert.That(okResult.Value, Is.Empty);
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

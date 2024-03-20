using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_GetOrders_Tests
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
                OrderTime = new DateTime(2024, 3, 18, 12, 29, 23),
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
        public async Task GetOrders_ReturnsCorrectNumberOfOrders()
        {
            //Arrange
            var expectedNumberOfOrders = 4;

            //Act
            var result = await _controller.GetOrders();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Has.Count.EqualTo(expectedNumberOfOrders));
        }

        [Test]
        public async Task GetOrders_ReturnsCorrectId([Values(0, 1, 2, 3)] int index)
        {
            //Act
            var result = await _controller.GetOrders();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Is.Not.Null);

            Assert.That(orders[index], Has.Property("OrderID").EqualTo(index + 1));
        }

        [Test]
        public async Task GetOrders_ReturnsCorrectTime([Values(0, 1, 2, 3)] int index)
        {
            //Arrange
            var expectedOrderDates = new List<DateTime>
            {
                new DateTime(2024, 3, 18, 12, 29, 23),
                new DateTime(2024, 3, 18, 12, 30, 00),
                new DateTime(2024, 3, 18, 12, 31, 00),
                new DateTime(2024, 3, 18, 12, 35, 00)
            };

            //Act
            var result = await _controller.GetOrders();
            
            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Is.Not.Null);

            Assert.That(orders[index], Has.Property("OrderTime").EqualTo(expectedOrderDates[index]));
        }

        [Test]
        public async Task GetOrders_ReturnsCorrectStatus([Values(0, 1, 2, 3)] int index)
        {
            //Arrange
            var expectedStatuses = new List<String>
            {
                "Pending",
                "Preparing",
                "Completed",
                "Delivered"
            };

            //Act
            var result = await _controller.GetOrders();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Is.Not.Null);

            Assert.That(orders[index], Has.Property("Status").EqualTo(expectedStatuses[index]));
        }

        [Test, Sequential]
        public async Task GetOrders_ReturnsCorrectGuestId([Values(0, 1, 2, 3)] int index, [Values(1, 1, 2, 2)] int gId)
        {
            //Act
            var result = await _controller.GetOrders();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Is.Not.Null);

            Assert.That(orders[index], Has.Property("GuestID").EqualTo(gId));
        }

        [Test]
        public async Task GetOrders_ReturnsEmptyList()
        {
            // Arrange
            _context.Orders.RemoveRange(_context.Orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetOrders();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orders = okResult.Value as List<OrderDTO>;

            Assert.That(orders, Is.Empty);
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

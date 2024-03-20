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
        public async Task GetOrder_ReturnsCorrectOrderID()
        {
            //Arrange
            var existingOrderId = 1;

            //Act
            var result = await _controller.GetOrder(existingOrderId);

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orderDTO = okResult.Value as OrderDTO;

            Assert.That(orderDTO, Has.Property("OrderID").EqualTo(existingOrderId));
        }

        [Test]
        public async Task GetOrder_ReturnsCorrectOrderTime()
        {
            //Arrange
            var existingOrderId = 2;
            var expectedDateTime = new DateTime(2024, 3, 18, 12, 30, 00);

            //Act
            var result = await _controller.GetOrder(existingOrderId);

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orderDTO = okResult.Value as OrderDTO;

            Assert.That(orderDTO, Has.Property("OrderTime").EqualTo(expectedDateTime));
        }

        [Test]
        public async Task GetOrder_ReturnsCorrectStatus()
        {
            //Arrange
            var existingOrderId = 3;

            //Act
            var result = await _controller.GetOrder(existingOrderId);

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orderDTO = okResult.Value as OrderDTO;

            Assert.That(orderDTO, Has.Property("Status").EqualTo("Completed"));
        }

        [Test]
        public async Task GetOrder_ReturnsCorrectGuestID()
        {
            //Arrange
            var existingOrderId = 4;
            var exptectedGuestId = 2;

            //Act
            var result = await _controller.GetOrder(existingOrderId);

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var orderDTO = okResult.Value as OrderDTO;

            Assert.That(orderDTO, Has.Property("GuestID").EqualTo(exptectedGuestId));
        }

        [Test]
        public async Task GetOrder_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            int nonExistingOrderId = 999;

            // Act
            var result = await _controller.GetOrder(nonExistingOrderId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Order with given ID doesn't exist"));
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

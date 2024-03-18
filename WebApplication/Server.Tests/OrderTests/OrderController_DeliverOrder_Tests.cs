using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_DeliverOrder_Tests
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
        public async Task DeliverOrder_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var existingOrderId = 3;
            var existingWaiterId = 2;

            // Act
            var result = await _controller.DeliverOrder(existingOrderId, existingWaiterId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeliverOrder_WithNonExistingOrderId_ReturnsNotFound()
        {
            //Arrange
            var nonExistingOrderId = 999;
            var existingWaiterId = 2;
            
            //Act
            var result = await _controller.DeliverOrder(nonExistingOrderId, existingWaiterId);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);

            Assert.That(notFoundResult.Value, Is.EqualTo("Order with given ID doesn't exist"));
        }

        [Test]
        public async Task DeliverOrder_WithNonExistingWaiterId_ReturnsNotFound()
        {
            //Arrange
            var existingOrderId = 3;
            var nonExistingWaiterId = 999;

            //Act
            var result = await _controller.DeliverOrder(existingOrderId, nonExistingWaiterId);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);

            Assert.That(notFoundResult.Value, Is.EqualTo("Waiter with given ID doesn't exist"));
        }

        [Test]
        public async Task DeliverOrder_WithNotCompletedOrder_ReturnsBadRequest()
        {
            //Arrange
            var existingOrderId = 1;
            var existingWaiterId = 2;

            //Act
            var result = await _controller.DeliverOrder(existingOrderId, existingWaiterId);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;

            Assert.That(badRequest, Is.Not.Null);

            Assert.That(badRequest.Value, Is.EqualTo("Order is not completed"));
        }

        [Test]
        public async Task DeliverOrder_UpdatesStatusCorrectly()
        {
            //Arrange
            var existingOrderId = 3;
            var existingWaiterId = 2;

            //Act
            await _controller.DeliverOrder(existingOrderId, existingWaiterId);

            //Assert
            var updatedOrder = await _context.Orders.FindAsync(existingOrderId);
            Assert.That(updatedOrder, Is.Not.Null);

            Assert.That(updatedOrder.Status, Is.EqualTo("Delivered"));
        }

        [Test]
        public async Task DeliverOrder_WaiterIsCorrect()
        {
            //Arrange
            var existingOrderId = 2;
            var existingWaiterId = 1;

            //Act
            await _controller.DeliverOrder(existingOrderId, existingWaiterId);

            //Assert
            var updatedOrder = await _context.Orders.FindAsync(existingOrderId);
            Assert.That(updatedOrder, Is.Not.Null);

            Assert.That(updatedOrder.WaiterID, Is.EqualTo(1));
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

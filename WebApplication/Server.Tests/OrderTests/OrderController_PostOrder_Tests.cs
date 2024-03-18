using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderTests
{
    public class OrderController_PostOrder_Tests
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

            // Setup the transaction just in case
            _transaction = _context.Database.BeginTransaction();
        }

        [Test]
        public async Task PostOrder_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var newOrder = new OrderCreateDTO { GuestID = 1, Quantity = 5, MenuItemID = 2 };

            // Act
            var result = await _controller.PostOrder(newOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task PostOrder_WithNegativeQuantity_ReturnsBadRequest()
        {
            // Arrange
            var newOrder = new OrderCreateDTO { GuestID = 1, Quantity = -1, MenuItemID = 2 };

            // Act
            var result = await _controller.PostOrder(newOrder);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Quantity must be a positive value"));
        }

        [Test]
        public async Task PostOrder_WithInvalidModelState_ReturnsBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("Erorr", "Some error 😭");
            var newOrder = new OrderCreateDTO();

            //Act
            var result = await _controller.PostOrder(newOrder);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
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

using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WaiterTests
{
    public class WaiterController_PostWaiter_Tests
    {
        private static PubContext _context;
        private static WaiterController _controller;
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

            _controller = new WaiterController(_context, _mapper);

            // Seed the database
            _context.Waiters.Add(new Waiter { WaiterID = 1, Name = "Waiter 1", Tips = 100 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 2, Name = "Waiter 2", Tips = 202 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 3, Name = "Waiter 3", Tips = 330 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 4, Name = "Waiter 4", Tips = 444 });
            _context.SaveChanges();

            // Setup the transaction just in case
            _transaction = _context.Database.BeginTransaction();
        }

        [Test]
        public async Task PostWaiter_WithValidData_ReturnsCreatedAtActionResult()
        {
            //Arrange
            var newWaiter = new WaiterDTO { WaiterID = 5, Name = "Cool short name 😎", Tips = 0 };

            //Act
            var result = await _controller.PostWaiter(newWaiter);

            //Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task PostWaiter_WithNameTooLong_ReturnsBadRequest()
        {
            //Arrange
            var newWaiter = new WaiterDTO { WaiterID = 5, Name = "ANOTHEER VEEEERY VEERY VEEEERY LOOOOOOOOOOOOOOONG NAMEEEE", Tips = 0 };

            //Act
            var result = await _controller.PostWaiter(newWaiter);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Name can't have more than 50 characters"));
        }

        [Test]
        public async Task PostWaiter_WithSetTips_ReturnsBadRequest()
        {
            //Arrange
            var newWaiter = new WaiterDTO { WaiterID = 5, Name = "New Name", Tips = 1 };

            //Act
            var result = await _controller.PostWaiter(newWaiter);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Tips can't be set"));
        }

        [Test]
        public async Task PostWaiter_WithInvalidModelState_ReturnsBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("Erorr", "Some error that will most likley happen to me 😢");
            var newWaiter = new WaiterDTO();

            //Act
            var result = await _controller.PostWaiter(newWaiter);

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

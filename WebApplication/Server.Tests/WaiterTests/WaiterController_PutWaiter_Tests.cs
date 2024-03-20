using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WaiterTests
{
    public class WaiterController_PutWaiter_Tests
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
        public async Task PutWaiter_WithValidData_ReturnsNoContentResult()
        {
            //Arrange
            var existingWaiterId = 4;
            var waiterDTO = new WaiterDTO { WaiterID = 4, Name = "New Name", Tips = 444 };

            //Act
            var result = await _controller.PutWaiter(existingWaiterId, waiterDTO);

            //Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PutWaiter_WithNameTooLong_ReturnsBadRequest()
        {
            //Arrange
            var existingWaiterId = 4;
            var waiterDTO = new WaiterDTO { WaiterID = 4, Name = "This is a VERY VEERY VEEEERY LOOOOOOOOOOOOOOONG NAMEEEE", Tips = 444 };

            //Act
            var result = await _controller.PutWaiter(existingWaiterId, waiterDTO);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Name can't have more than 50 characters"));
        }

        [Test]
        public async Task PutWaiter_WithNegativeTips_ReturnsBadRequest()
        {
            //Arrange
            var existingWaiterId = 4;
            var waiterDTO = new WaiterDTO { WaiterID = 4, Name = "New Name", Tips = -444 };

            //Act
            var result = await _controller.PutWaiter(existingWaiterId, waiterDTO);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Tips can't be negative"));
        }

        [Test]
        public async Task PutWaiter_UpdatesNameCorrectly()
        {
            //Arrange
            var existingWaiterId = 4;
            var waiterDTO = new WaiterDTO { WaiterID = 4, Name = "New Name", Tips = 444 };

            //Act
            await _controller.PutWaiter(existingWaiterId, waiterDTO);

            //Assert
            var updatedWaiter = await _context.Waiters.FindAsync(existingWaiterId);
            Assert.That(updatedWaiter, Is.Not.Null);

            Assert.That(updatedWaiter.Name, Does.StartWith("new").IgnoreCase);
        }

        [Test]
        public async Task PutWaiter_UpdatesTipsCorrectly()
        {
            //Arrange
            var existingWaiterId = 4;
            var waiterDTO = new WaiterDTO { WaiterID = 4, Name = "Waiter 4", Tips = 1234 };

            //Act
            await _controller.PutWaiter(existingWaiterId, waiterDTO);

            //Assert
            var updatedWaiter = await _context.Waiters.FindAsync(existingWaiterId);
            Assert.That(updatedWaiter, Is.Not.Null);

            Assert.That(updatedWaiter.Tips, Is.GreaterThan(1233));
        }

        [Test]
        public async Task PutWaiter_WithMismatchingIds_ReturnsBadRequest()
        {
            //Arrange
            var id_1 = 1;
            var id_2 = 2;
            var waiterDTO = new WaiterDTO { WaiterID = id_2, Name = "New Name", Tips = 1234 };

            //Act
            var result = await _controller.PutWaiter(id_1, waiterDTO);

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Has.Property("Value").EqualTo("Waiter IDs don't match"));
        }

        [Test]
        public async Task PutWaiter_WithNonExistingWaiter_ReturnsNotFound()
        {
            //Arrange
            var nonExistingId = 999;
            var waiterDTO = new WaiterDTO { WaiterID = nonExistingId, Name = "New Name", Tips = 1234 };

            //Act
            var result = await _controller.PutWaiter(nonExistingId, waiterDTO);

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

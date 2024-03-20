using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WaiterTests
{
    public class WaiterController_GetWaiter_Tests
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
        public async Task GetWaiter_WithExistingId_ReturnsWaiter()
        {
            //Arrange
            var existingWaiterId = 4;

            //Act
            var result = await _controller.GetWaiter(existingWaiterId);

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waiterDTO = okResult.Value as WaiterDTO;

            Assert.That(waiterDTO, Is.Not.Null);

            Assert.That(waiterDTO, Has.Property("WaiterID").EqualTo(existingWaiterId));
        }

        [Test]
        public async Task GetWaiter_WithNonExistingId_ReturnsNotFound()
        {
            //Arrange
            var nonExistingWaiterId = 999;

            //Act
            var result = await _controller.GetWaiter(nonExistingWaiterId);

            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);

            Assert.That(notFoundResult, Has.Property("Value").EqualTo("Waiter with given ID doesn't exist"));
        }

        [Test]
        public async Task GetWaiter_WithExistingId_ReturnsCorrectName()
        {
            //Arrange
            var existingWaiterId = 3;
            var existingWaiterName = "Waiter 3";

            //Act
            var result = await _controller.GetWaiter(existingWaiterId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waiterDTO = okResult.Value as WaiterDTO;

            Assert.That(waiterDTO, Is.Not.Null);

            Assert.That(waiterDTO, Has.Property("Name").EqualTo(existingWaiterName));
        }

        [Test]
        public async Task GetWaiter_WithExistingId_ReturnsCorrectTips()
        {
            //Arrange
            var existingWaiterId = 2;
            var existingWaiterTips = 202;

            //Act
            var result = await _controller.GetWaiter(existingWaiterId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waiterDTO = okResult.Value as WaiterDTO;

            Assert.That(waiterDTO, Is.Not.Null);

            Assert.That(waiterDTO, Has.Property("Tips").EqualTo(existingWaiterTips));
        }

        [Test]
        public async Task GetWaiter_WithExistingId_ReturnsCompleteAndAccurateData()
        {
            //Arrange
            var existingWaiterId = 1;
            var existingWaiterName = "Waiter 1";
            var existingWaiterTips = 100;

            //Act
            var result = await _controller.GetWaiter(existingWaiterId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waiterDTO = okResult.Value as WaiterDTO;

            Assert.That(waiterDTO, Is.Not.Null);

            Assert.That(waiterDTO, Has.Property("WaiterID").EqualTo(existingWaiterId));
            Assert.That(waiterDTO, Has.Property("Name").EqualTo(existingWaiterName));
            Assert.That(waiterDTO, Has.Property("Tips").EqualTo(existingWaiterTips));
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

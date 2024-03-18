using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WaiterTests
{
    public class WaiterController_GetWaiters_Tests
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
            _context.Waiters.Add(new Waiter { WaiterID = 2, Name = "Waiter 2", Tips = 200 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 3, Name = "Waiter 3", Tips = 300 });
            _context.SaveChanges();
            _context.Waiters.Add(new Waiter { WaiterID = 4, Name = "Waiter 4", Tips = 400 });
            _context.SaveChanges();

            // Setup the transaction just in case
            _transaction = _context.Database.BeginTransaction();
        }

        [Test]
        public async Task GetWaiters_ReturnsCorrectNumberOfWaiters()
        {
            // Act
            var result = await _controller.GetWaiters();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waitersList = okResult.Value as List<WaiterDTO>;

            Assert.That(waitersList, Is.Not.Null);
            Assert.That(waitersList, Has.Count.EqualTo(4));
        }

        [Test]
        public async Task GetWaiters_AllWaiters_HaveCorrectID([Values(1, 2, 3, 4)] int id)
        {
            // Act
            var result = await _controller.GetWaiters();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waitersList = okResult.Value as List<WaiterDTO>;

            Assert.That(waitersList, Is.Not.Null);

            Assert.That(waitersList[id - 1].WaiterID, Is.EqualTo(id));
        }

        [Test, Sequential]
        public async Task GetWaiters_AllWaiters_HaveCorrectNameAndTips(
            [Values(1, 2, 3, 4)] int id, 
            [Values("Waiter 1", "Waiter 2", "Waiter 3", "Waiter 4")] string name
            )
        {
            // Act
            var result = await _controller.GetWaiters();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waitersList = okResult.Value as List<WaiterDTO>;

            Assert.That(waitersList, Is.Not.Null);

            Assert.That(waitersList[id - 1].Name, Is.EqualTo(name));
            Assert.That(waitersList[id - 1].Tips, Is.EqualTo(id * 100));
        }

        [Test]
        public async Task GetWaiters_EveryWaiterIsUnique()
        {
            // Act
            var result = await _controller.GetWaiters();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waitersList = okResult.Value as List<WaiterDTO>;

            Assert.That(waitersList, Is.Not.Null);

            Assert.That(waitersList, Is.Unique);
        }

        [Test]
        public async Task GetWaiters_WhenNoWaitersExist_ReturnsEmptyList()
        {
            //Arrange
            _context.Waiters.RemoveRange(_context.Waiters);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetWaiters();

            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var waitersList = okResult.Value as List<WaiterDTO>;

            Assert.That(waitersList, Is.Not.Null);

            Assert.That(waitersList, Is.Empty);
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

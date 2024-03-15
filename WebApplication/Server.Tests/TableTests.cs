using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using Server.Models;

namespace Server.Tests
{
    public class TableTests
    {
        private IMapper _mapper;
        private TableController _tableController;
        private PubContext _context;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Initialize AutoMapper only once for all tests, as it doesn't change per test case.
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile()); // Your profile here
            });
            _mapper = mockMapper.CreateMapper();
        }

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var connectionString = configuration.GetConnectionString("PubDB");

            var options = new DbContextOptionsBuilder<PubContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new PubContext(options);

            _tableController = new TableController(_context, _mapper);
        }

        [Test]
        public async Task GetTables_ShouldReturnAllTables()
        {
            // Arrange
            // Seed the database with test data
            SeedDatabaseWithTestData();

            // Act
            var actionResult = await _tableController.GetTables();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult>());

            var result = actionResult as OkObjectResult;
            Assert.That(result, Is.Not.Null);

            var tables = result.Value as List<TableDTO>;
            Assert.That(tables, Is.Not.Null, "Expected non-null List<TableDTO>");

            // Assuming you seeded 3 tables
            Assert.That(tables.Count, Is.EqualTo(3), "Expected 3 tables");
        }

        [TearDown]
        public void TearDown()
        {
            // Assuming that the _context is the DbContext for your database and
            // it is properly instantiated and available here.

            // Remove any Tables entities that were added during the test.
            _context.Tables.RemoveRange(_context.Tables);
            _context.SaveChanges();

            // Dispose of the context if you are not using dependency injection's built-in disposal.
            _context.Dispose();
        }

        private void SeedDatabaseWithTestData()
        {
            _context.Tables.RemoveRange(_context.Tables);
            _context.SaveChanges();

            var testTables = new List<Table>
            {
                new Table { Number = 1, Seats = 4, Status = "Available" },
                new Table { Number = 2, Seats = 2, Status = "Occupied" },
                new Table { Number = 3, Seats = 6, Status = "Reserved" }
            };

            _context.Tables.AddRange(testTables);
            _context.SaveChanges();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TableTests;

[TestFixture]
public class TableController_GetTables_Tests
{
    private static PubContext _context;
    private static TableController _controller;
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

        _controller = new TableController(_context, _mapper);

        // Seed the database
        _context.Tables.Add(new Table { TableID = 1, Number = 101, Seats = 4, Status = "Available" });
        _context.SaveChanges();
        _context.Tables.Add(new Table { TableID = 2, Number = 102, Seats = 6, Status = "Occupied" });
        _context.SaveChanges();
        _context.Tables.Add(new Table { TableID = 3, Number = 103, Seats = 6, Status = "Full" });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task GetTables_ReturnsAllTables()
    {
        // Act
        var result = await _controller.GetTables();

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tables = okResult.Value as List<TableDTO>;
        Assert.That(tables, Is.Not.Null);
        Assert.That(tables, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task GetTables_ReturnsNumberProperties([Values(0, 1, 2)] int index)
    {
        // Act
        var result = await _controller.GetTables();

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tables = okResult.Value as List<TableDTO>;
        Assert.That(tables, Is.Not.Null);
        Assert.That(tables, Has.Count.EqualTo(3));

        Assert.That(tables[index], Has.Property("Number").EqualTo(100 + index + 1));
    }

    [Test]
    public async Task GetTables_ReturnsSeatsProperties([Values(0, 1, 2)] int index)
    {
        // Arrange
        var expectedResults = new List<int> { 4, 6 , 6 };

        // Act
        var result = await _controller.GetTables();

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tables = okResult.Value as List<TableDTO>;
        Assert.That(tables, Is.Not.Null);
        Assert.That(tables, Has.Count.EqualTo(3));

        Assert.That(tables[index], Has.Property("Seats").EqualTo(expectedResults[index]));
    }

    [Test]
    public async Task GetTables_ReturnsStatusProperties([Values(0, 1, 2)] int index)
    {
        // Arrange
        var expectedResults = new List<string> { "Available", "Occupied", "Full" };

        // Act
        var result = await _controller.GetTables();

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tables = okResult.Value as List<TableDTO>;
        Assert.That(tables, Is.Not.Null);
        Assert.That(tables, Has.Count.EqualTo(3));

        Assert.That(tables[index], Has.Property("Status").EqualTo(expectedResults[index]));
    }

    [Test]
    public async Task GetTables_ReturnsEmptyList()
    {
        // Arrange
        _context.Tables.RemoveRange(_context.Tables);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTables();

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tables = okResult.Value as List<TableDTO>;
        Assert.That(tables, Is.Not.Null);
        Assert.That(tables, Has.Count.EqualTo(0));
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
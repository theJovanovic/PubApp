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
    public async Task GetTables_ReturnsNumberProperties()
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

        Assert.That(tables[0], Has.Property("Number").EqualTo(101));
        Assert.That(tables[1], Has.Property("Number").EqualTo(102));
        Assert.That(tables[2], Has.Property("Number").EqualTo(103));
    }

    [Test]
    public async Task GetTables_ReturnsSeatsProperties()
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

        Assert.That(tables[0], Has.Property("Seats").EqualTo(4));
        Assert.That(tables[1], Has.Property("Seats").EqualTo(6));
        Assert.That(tables[2], Has.Property("Seats").EqualTo(6));
    }

    [Test]
    public async Task GetTables_ReturnsStatusProperties()
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

        Assert.That(tables[0], Has.Property("Status").EqualTo("Available"));
        Assert.That(tables[1], Has.Property("Status").EqualTo("Occupied"));
        Assert.That(tables[2], Has.Property("Status").EqualTo("Full"));
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
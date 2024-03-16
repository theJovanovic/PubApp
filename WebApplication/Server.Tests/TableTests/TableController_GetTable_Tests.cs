using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TableTests;

[TestFixture]
public class TableController_GetTable_Tests
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
        _context.Tables.Add(new Table { TableID = 2, Number = 102, Seats = 6, Status = "Occupied" });
        _context.Tables.Add(new Table { TableID = 3, Number = 103, Seats = 6, Status = "Full" });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task GetTable_WithExistingId_ReturnsTable()
    {
        // Arrange
        int testTableId = 1;

        // Act
        var result = await _controller.GetTable(testTableId);

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tableDTO = okResult.Value as TableDTO;
        Assert.That(tableDTO, Is.Not.Null);
        Assert.That(tableDTO, Has.Property("TableID").EqualTo(testTableId));
    }

    [Test]
    public async Task GetTable_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingTableId = 0;

        // Act
        var result = await _controller.GetTable(nonExistingTableId);

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult, Has.Property("StatusCode").EqualTo(404));
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Table with given ID doesn't exist"));
    }

    [Test]
    public async Task GetTable_WithExistingId_ReturnsCompleteAndAccurateData()
    {
        // Arrange
        int testTableId = 1;

        // Act
        var result = await _controller.GetTable(testTableId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        var tableDTO = okResult.Value as TableDTO;
        Assert.That(tableDTO, Is.Not.Null);
        Assert.That(tableDTO, Has.Property("TableID").EqualTo(testTableId));
        Assert.That(tableDTO, Has.Property("Number").EqualTo(101));
        Assert.That(tableDTO, Has.Property("Seats").EqualTo(4));
        Assert.That(tableDTO, Has.Property("Status").EqualTo("Available"));
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

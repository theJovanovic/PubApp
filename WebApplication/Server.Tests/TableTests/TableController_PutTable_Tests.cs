using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TableTests;

[TestFixture]
public class TableController_PutTable_Tests
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
    public async Task PutTable_WithInvalidTableNumber_ReturnsBadRequest()
    {
        var tableDTO = new TableDTO { TableID = 1, Number = 0, Seats = 4, Status = "Available" };

        var result = await _controller.PutTable(1, tableDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table number must be a positive value"));
    }

    [Test]
    public async Task PutTable_WithInvalidSeats_ReturnsBadRequest()
    {
        var tableDTO = new TableDTO { TableID = 1, Number = 5, Seats = 0, Status = "Available" };

        var result = await _controller.PutTable(1, tableDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Seats must be a positive value"));
    }

    [Test]
    public async Task PutTable_WithMismatchedIds_ReturnsBadRequest()
    {
        var id_1 = 1;
        var id_2 = 2;
        var tableDTO = new TableDTO { TableID = id_2, Number = 5, Seats = 4, Status = "Available" };

        var result = await _controller.PutTable(id_1, tableDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table IDs doesn't match"));
    }

    [Test]
    public async Task PutTable_WithNonExistingTable_ReturnsNotFound()
    {
        var nonExistingId = 0;
        var tableDTO = new TableDTO { TableID = nonExistingId, Number = 5, Seats = 4, Status = "Available" };

        var result = await _controller.PutTable(nonExistingId, tableDTO);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Table with given ID doesn't exist"));
    }

    [Test]
    public async Task PutTable_WithExistingNumberOnDifferentTable_ReturnsConflict()
    {
        var existingTableNumber = 102;
        var tableDTO = new TableDTO { TableID = 1, Number = existingTableNumber, Seats = 4, Status = "Available" };

        var result = await _controller.PutTable(1, tableDTO);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        var conflictResult = result as ConflictObjectResult;
        Assert.That(conflictResult, Has.Property("Value").EqualTo("Table with the same number already exists"));
    }

    [Test]
    public async Task PutTable_WithValidData_ReturnsNoContent()
    {
        var tableDTO = new TableDTO { TableID = 3, Number = 104, Seats = 6, Status = "Full" };

        var result = await _controller.PutTable(3, tableDTO);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
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
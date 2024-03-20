using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TableTests;

[TestFixture]
public class TableController_PostTable_Tests
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
    public async Task PostTable_WithValidInput_ReturnsCreatedAtAction()
    {
        // Arrange
        var tableDTO = new TableDTO { TableID = 4, Number = 104, Seats = 4, Status = "Available" };

        // Act
        var result = await _controller.PostTable(tableDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var createdAtResult = result as CreatedAtActionResult;
        Assert.That(createdAtResult, Has.Property("Value").InstanceOf<TableDTO>());
        var resultDTO = createdAtResult.Value as TableDTO;
        Assert.That(resultDTO, Has.Property("Number").EqualTo(tableDTO.Number));
    }

    [Test]
    public async Task PostTable_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");
        var tableDTO = new TableDTO();

        // Act
        var result = await _controller.PostTable(tableDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PostTable_WithNonPositiveTableNumber_ReturnsBadRequest()
    {
        // Arrange
        var tableDTO = new TableDTO { TableID = 4, Number = 0, Seats = 4, Status = "Available" };

        // Act
        var result = await _controller.PostTable(tableDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table number must be a positive value"));
    }

    [Test]
    public async Task PostTable_WithNonPositiveSeats_ReturnsBadRequest()
    {
        // Arrange
        var tableDTO = new TableDTO { TableID = 4, Number = 104, Seats = 0, Status = "Available" };

        // Act
        var result = await _controller.PostTable(tableDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Seats must be a positive value"));
    }

    [Test]
    public async Task PostTable_WithDuplicateTableNumber_ReturnsConflict()
    {
        // Arrange
        var tableDTO = new TableDTO { TableID = 4, Number = 101, Seats = 4, Status = "Available" };

        // Act
        var result = await _controller.PostTable(tableDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        var conflictResult = result as ConflictObjectResult;
        Assert.That(conflictResult, Has.Property("Value").EqualTo("Table with the same number already exist"));
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
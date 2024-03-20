using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TableTests;

[TestFixture]
public class TableController_GetTableInfo_Tests
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
    public async Task GetTableInfo_WithExistingIdAndNoGuests_ReturnsCorrectInfo()
    {
        // Arrange
        int testTableId = 1;

        // Act
        var result = await _controller.GetTableInfo(testTableId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        dynamic tableInfo = okResult.Value;
        Assert.That(tableInfo, Has.Property("TableID").EqualTo(testTableId));
        Assert.That(tableInfo, Has.Property("Guests").Count.EqualTo(0));
    }

    [Test]
    public async Task GetTableInfo_WithExistingIdAndGuests_ReturnsCorrectInfoAndGuests()
    {
        // Arrange
        int testTableId = 2;
        _context.Guests.Add(new Guest { 
            GuestID = 1, 
            Name = "Guest 1", 
            HasAllergies = false, 
            HasDiscount = true, 
            Money = 1200, 
            TableID = testTableId
        });
        _context.SaveChanges();

        // Act
        var result = await _controller.GetTableInfo(testTableId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult, Has.Property("StatusCode").EqualTo(200));

        dynamic tableInfo = okResult.Value;
        Assert.That(tableInfo, Has.Property("TableID").EqualTo(testTableId));
        Assert.That(tableInfo, Has.Property("Guests").Count.EqualTo(1));
    }

    [Test]
    public async Task GetTableInfo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingTableId = 0;

        // Act
        var result = await _controller.GetTableInfo(nonExistingTableId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult, Has.Property("StatusCode").EqualTo(404));
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Table with given ID doesn't exist"));
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

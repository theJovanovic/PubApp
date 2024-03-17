using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_PostGuest_Tests
{
    private static PubContext _context;
    private static GuestController _controller;
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

        _controller = new GuestController(_context, _mapper);

        // Seed the database
        _context.Tables.Add(new Table { TableID = 1, Number = 1, Seats = 1, Status = "Full" });
        _context.SaveChanges();
        _context.Tables.Add(new Table { TableID = 2, Number = 2, Seats = 2, Status = "Occupied" });
        _context.SaveChanges();
        _context.Tables.Add(new Table { TableID = 3, Number = 3, Seats = 3, Status = "Available" });
        _context.SaveChanges();

        _context.Guests.Add(new Guest
        {
            GuestID = 1,
            HasAllergies = false,
            HasDiscount = false,
            Money = 1200,
            Name = "Guest 1",
            TableID = 1
        });
        _context.SaveChanges();
        _context.Guests.Add(new Guest
        {
            GuestID = 2,
            HasAllergies = true,
            HasDiscount = false,
            Money = 2200,
            Name = "Guest 2",
            TableID = 2
        });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task PostGuest_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some model state error");
        var guestDTO = new GuestDTO();

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PostGuest_WithNameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var guestDTO = new GuestDTO { Name = new string('A', 51) };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Name can't have more than 50 characters"));
    }

    [Test]
    public async Task PostGuest_WithNegativeMoney_ReturnsBadRequest()
    {
        // Arrange
        var guestDTO = new GuestDTO { Name = "Guest", Money = -100 };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Money can't be negative"));
    }

    [Test]
    public async Task PostGuest_WithInvalidTableNumber_ReturnsBadRequest()
    {
        // Arrange
        var guestDTO = new GuestDTO { Name = "Guest", TableNumber = 0 };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table number must be positive value"));
    }

    [Test]
    public async Task PostGuest_ToNonExistingTable_ReturnsNotFound()
    {
        // Arrange
        var guestDTO = new GuestDTO { Name = "Guest", TableNumber = 999 };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Table with given number doesn't exist"));
    }

    [Test]
    public async Task PostGuest_ToFullTable_ReturnsConflict()
    {
        // Arrange
        var guestDTO = new GuestDTO { Name = "Guest", TableNumber = 1 };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        var conflictResult = result as ConflictObjectResult;
        Assert.That(conflictResult, Has.Property("Value").EqualTo("Table is already full"));
    }

    [Test]
    public async Task PostGuest_ToOccupiedTable_UpdatesTableStatusToFull()
    {
        // Arrange
        var tableNumber = 2;
        var guestDTO = new GuestDTO { Name = "Guest", TableNumber = tableNumber };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

        var table = _context.Tables.FirstOrDefault(t => t.Number == tableNumber);

        Assert.That(table, Has.Property("Status").EqualTo("Full"));
    }

    [Test]
    public async Task PostGuest_ToAvailableTable_UpdatesTableStatusToOccupied()
    {
        // Arrange
        var tableNumber = 2;
        var guestDTO = new GuestDTO { Name = "Guest", TableNumber = tableNumber };

        // Act
        var result = await _controller.PostGuest(guestDTO);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

        var table = _context.Tables.FirstOrDefault(t => t.Number == tableNumber);
        Assert.That(table, Has.Property("Status").EqualTo("Full"));
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
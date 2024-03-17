using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_PutGuest_Tests
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
    public async Task PutGuest_WithNameTooLong_ReturnsBadRequest()
    {
        var guestDTO = new GuestDTO { GuestID = 1, Name = new string('A', 51), Money = 100, TableNumber = 1 };

        var result = await _controller.PutGuest(1, guestDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Name can't have more than 50 characters"));
    }

    [Test]
    public async Task PutGuest_WithNegativeMoney_ReturnsBadRequest()
    {
        var guestDTO = new GuestDTO { GuestID = 1, Name = "John Doe", Money = -1, TableNumber = 1 };

        var result = await _controller.PutGuest(1, guestDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Money can't be negative"));
    }

    [Test]
    public async Task PutGuest_WithInvalidTableNumber_ReturnsBadRequest()
    {
        var guestDTO = new GuestDTO { GuestID = 1, Name = "John Doe", Money = 100, TableNumber = 0 };

        var result = await _controller.PutGuest(1, guestDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table number must be positive value"));
    }

    [Test]
    public async Task PutGuest_WithMismatchedIds_ReturnsBadRequest()
    {
        var guestDTO = new GuestDTO { GuestID = 2, Name = "John Doe", Money = 100, TableNumber = 1 };

        var result = await _controller.PutGuest(1, guestDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Guest IDs don't match"));
    }

    [Test]
    public async Task PutGuest_ForNonExistingGuest_ReturnsNotFound()
    {
        var guestDTO = new GuestDTO { GuestID = 999, Name = "John Doe", Money = 100, TableNumber = 1 };

        var result = await _controller.PutGuest(999, guestDTO);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
    }

    [Test]
    public async Task PutGuest_ToNonExistingTable_ReturnsNotFound()
    {
        var guestDTO = new GuestDTO { GuestID = 1, Name = "John Doe", Money = 100, TableNumber = 999 };

        var result = await _controller.PutGuest(1, guestDTO);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Table with given table number doesn't exist"));
    }

    [Test]
    public async Task PutGuest_ToFullTable_ReturnsBadRequest()
    {
        var guestDTO = new GuestDTO { GuestID = 2, Name = "John Doe", Money = 100, TableNumber = 1 };

        var result = await _controller.PutGuest(2, guestDTO);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Table is already full"));
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
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_GetGuests_Tests
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

        _context.Guests.Add(new Guest { 
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
    public async Task GetGuests_ReturnsCorrectNumberOfGuests()
    {
        // Act
        var result = await _controller.GetGuests();

        // Assert
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectID()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("GuestID").EqualTo(1));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectName()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("Name").EqualTo("Guest 1"));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectMoney()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("Money").EqualTo(1200));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasAllergiesCorrectlySet()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("HasAllergies").EqualTo(false));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasDiscountCorrectlySet()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("HasDiscount").EqualTo(false));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectTableNumber()
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[0], Has.Property("TableNumber").EqualTo(1));
    }


    [Test]
    public async Task GetGuests_WhenNoGuestsExist_ReturnsEmptyList()
    {
        // Arrange
        _context.Guests.RemoveRange(_context.Guests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetGuests();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.InstanceOf<List<GuestDTO>>());
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList, Is.Empty);
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
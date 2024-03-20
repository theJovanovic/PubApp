using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json.Linq;

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
            HasDiscount = true,
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
    public async Task GetGuests_FirstGuest_HasCorrectID([Values(0, 1)] int index)
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("GuestID").EqualTo(index + 1));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectName([Values(0, 1)] int index)
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("Name").EqualTo($"Guest {index+1}"));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectMoney([Values(0, 1)] int index)
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("Money").EqualTo(200 + (index+1)*1000));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasAllergiesCorrectlySet([Values(0, 1)] int index)
    {
        var expectedResults = new List<bool> { false, true };
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("HasAllergies").EqualTo(expectedResults[index]));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasDiscountCorrectlySet([Values(0, 1)] int index)
    {
        var expectedResults = new List<bool> { false, true };
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("HasDiscount").EqualTo(expectedResults[index]));
    }

    [Test]
    public async Task GetGuests_FirstGuest_HasCorrectTableNumber([Values(0, 1)] int index)
    {
        var result = await _controller.GetGuests();
        var okResult = result as OkObjectResult;
        var guestsList = okResult.Value as List<GuestDTO>;
        Assert.That(guestsList[index], Has.Property("TableNumber").EqualTo(index+1));
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
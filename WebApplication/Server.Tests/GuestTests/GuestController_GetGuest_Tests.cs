using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_GetGuest_Tests
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
    public async Task GetGuest_WithExistingId_ReturnsGuest()
    {
        // Arrange
        var existingGuestId = 1;
        var existingGuestName = "Guest 1";
        var existingGuestMoney = 1200;

        // Act
        var result = await _controller.GetGuest(existingGuestId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.InstanceOf<GuestDTO>());
        var guestDTO = okResult.Value as GuestDTO;
        Assert.That(guestDTO, Has.Property("Name").EqualTo(existingGuestName));
        Assert.That(guestDTO, Has.Property("Money").EqualTo(existingGuestMoney));
    }

    [Test]
    public async Task GetGuest_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingGuestId = 0;

        // Act
        var result = await _controller.GetGuest(nonExistingGuestId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
    }

    [Test]
    public async Task GetGuest_WithExistingId_ReturnsCorrectName()
    {
        var existingGuestId = 1;
        var existingGuestName = "Guest 1";

        var result = await _controller.GetGuest(existingGuestId);
        var okResult = result as OkObjectResult;
        var guestDTO = okResult.Value as GuestDTO;

        Assert.That(guestDTO, Has.Property("Name").EqualTo(existingGuestName));
    }

    [Test]
    public async Task GetGuest_WithExistingId_ReturnsCorrectMoney()
    {
        var existingGuestId = 1;
        var existingGuestMoney = 1200;

        var result = await _controller.GetGuest(existingGuestId);
        var okResult = result as OkObjectResult;
        var guestDTO = okResult.Value as GuestDTO;

        Assert.That(guestDTO, Has.Property("Money").EqualTo(existingGuestMoney));
    }

    [Test]
    public async Task GetGuest_WithExistingId_ReturnsCorrectHasDiscount()
    {
        var existingGuestId = 1;
        var existingGuestDiscount = false;

        var result = await _controller.GetGuest(existingGuestId);
        var okResult = result as OkObjectResult;
        var guestDTO = okResult.Value as GuestDTO;

        Assert.That(guestDTO, Has.Property("HasDiscount").EqualTo(existingGuestDiscount));
    }

    [Test]
    public async Task GetGuest_WithExistingId_ReturnsCorrectHasAllergies()
    {
        var existingGuestId = 1;
        var existingGuestAllergies = false;

        var result = await _controller.GetGuest(existingGuestId);
        var okResult = result as OkObjectResult;
        var guestDTO = okResult.Value as GuestDTO;

        Assert.That(guestDTO, Has.Property("HasAllergies").EqualTo(existingGuestAllergies));
    }

    [Test]
    public async Task GetGuest_WithExistingId_ReturnsCorrectTableID()
    {
        var existingGuestId = 1;
        var existingGuestTableNumber = 1;

        var result = await _controller.GetGuest(existingGuestId);
        var okResult = result as OkObjectResult;
        var guestDTO = okResult.Value as GuestDTO;

        Assert.That(guestDTO, Has.Property("TableNumber").EqualTo(existingGuestTableNumber));
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
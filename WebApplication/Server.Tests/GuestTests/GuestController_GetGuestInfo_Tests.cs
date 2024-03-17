using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_GetGuestInfo_Tests
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
            Name = "Guest 1",
            HasAllergies = false,
            HasDiscount = false,
            Money = 1200,
            TableID = 1
        });
        _context.SaveChanges();
        _context.Guests.Add(new Guest
        {
            GuestID = 2,
            Name = "Guest 2",
            HasAllergies = true,
            HasDiscount = false,
            Money = 2200,
            TableID = 2
        });
        _context.SaveChanges();

        _context.MenuItems.Add(new MenuItem
        {
            MenuItemID = 1,
            Name = "Item 1",
            Category = "Italian",
            HasAllergens = false,
            Price = 400
        });
        _context.SaveChanges();

        _context.Orders.Add(new Order
        {
            OrderID = 1,
            Quantity = 1,
            GuestID = 1,
            MenuItemID = 1,
        });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task GetGuestInfo_WithNonExistingId_ReturnsNotFound()
    {
        int nonExistingGuestId = 0;

        var result = await _controller.GetGuestInfo(nonExistingGuestId);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
    }

    [Test]
    public async Task GetGuestInfo_ExistingGuestWithoutOrders_ReturnsGuestInfo()
    {
        int existingGuestId = 2;

        var result = await _controller.GetGuestInfo(existingGuestId);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var guestInfo = okResult.Value;
        Assert.That(guestInfo, Has.Property("GuestID").EqualTo(existingGuestId));
        Assert.That(guestInfo, Has.Property("Orders").Count.EqualTo(0));
    }

    [Test]
    public async Task GetGuestInfo_ExistingGuestWithOrders_ReturnsGuestInfoIncludingOrders()
    {
        int existingGuestId = 1;

        var result = await _controller.GetGuestInfo(existingGuestId);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var guestInfo = okResult.Value;
        Assert.That(guestInfo, Has.Property("GuestID").EqualTo(existingGuestId));
        Assert.That(guestInfo, Has.Property("Orders").Count.EqualTo(1));
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
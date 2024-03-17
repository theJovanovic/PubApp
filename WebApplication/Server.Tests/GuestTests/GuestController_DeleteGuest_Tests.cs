using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GuestTests;

[TestFixture]
public class GuestController_DeleteGuest_Tests
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
    public async Task DeleteGuest_WithNonExistingId_ReturnsNotFound()
    {
        int nonExistingGuestId = 999;

        var result = await _controller.DeleteGuest(nonExistingGuestId);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
    }

    [Test]
    public async Task DeleteGuest_WithLastGuestAtTable_UpdatesTableStatusToAvailable()
    {
        // Arrange
        int guestIdToDelete = 1;

        var result = await _controller.DeleteGuest(guestIdToDelete);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var updatedTable = await _context.Tables.FindAsync(1);
        Assert.That(updatedTable.Status, Is.EqualTo("Available"));
    }

    [Test]
    public async Task DeleteGuest_WithGuestFromFullTable_UpdatesTableStatusToOccupied()
    {
        // Arrange
        int guestIdToDelete = 1;
        _context.Guests.Add(new Guest
        {
            GuestID = 3,
            HasAllergies = false,
            HasDiscount = false,
            Money = 1200,
            Name = "Guest 3",
            TableID = 2
        });
        _context.SaveChanges();

        var result = await _controller.DeleteGuest(guestIdToDelete);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var updatedTable = await _context.Tables.FindAsync(2);
        Assert.That(updatedTable.Status, Is.EqualTo("Occupied"));
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
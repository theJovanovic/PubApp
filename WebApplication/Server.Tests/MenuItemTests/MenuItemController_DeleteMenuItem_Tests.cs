using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;

namespace MenuItemTests;

[TestFixture]
public class MenuItemController_DeleteMenuItem_Tests
{
    private static PubContext _context;
    private static MenuItemController _controller;
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

        _controller = new MenuItemController(_context, _mapper);

        // Seed the database
        _context.MenuItems.Add(new MenuItem
        {
            MenuItemID = 1,
            Name = "Item 1",
            Category = "Category 1",
            HasAllergens = false,
            Price = 100
        });
        _context.SaveChanges();
        _context.MenuItems.Add(new MenuItem
        {
            MenuItemID = 2,
            Name = "Item 2",
            Category = "Category 2",
            HasAllergens = true,
            Price = 200
        });
        _context.SaveChanges();
        _context.MenuItems.Add(new MenuItem
        {
            MenuItemID = 3,
            Name = "Item 3",
            Category = "Category 3",
            HasAllergens = true,
            Price = 300
        });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task DeleteMenuItem_WithNonExistingId_ReturnsNotFound()
    {
        int nonExistingMenuItemId = 999;

        var result = await _controller.DeleteMenuItem(nonExistingMenuItemId);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value, Is.EqualTo("Item with given ID doesn't exist"));
    }

    [Test]
    public async Task DeleteMenuItem_WithExistingId_DeletesItemSuccessfully()
    {
        // Arrange
        int existingMenuItemId = 1;

        // Act
        var result = await _controller.DeleteMenuItem(existingMenuItemId);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        var deletedItem = await _context.MenuItems.FindAsync(existingMenuItemId);
        Assert.That(deletedItem, Is.Null);
    }

    [Test]
    public async Task DeleteMenuItem_WithExistingId_DecreasesItemCount()
    {
        // Arrange
        var initialCount = await _context.MenuItems.CountAsync();
        int existingMenuItemId = 1;

        // Act
        await _controller.DeleteMenuItem(existingMenuItemId);

        // Assert
        var newCount = await _context.MenuItems.CountAsync();
        Assert.That(newCount, Is.EqualTo(initialCount - 1));
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
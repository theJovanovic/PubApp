using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MenuItemTests;

[TestFixture]
public class MenuItemController_GetMenuItem_Tests
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
    public async Task GetMenuItem_ReturnsCorrectMenuItemID()
    {
        int existingMenuItemId = 1;

        var result = await _controller.GetMenuItem(existingMenuItemId);
        var okResult = result as OkObjectResult;
        var menuItemDTO = okResult.Value as MenuItemDTO;

        Assert.That(menuItemDTO, Has.Property("MenuItemID").EqualTo(existingMenuItemId));
    }

    [Test]
    public async Task GetMenuItem_ReturnsCorrectName()
    {
        int existingMenuItemId = 1;

        var result = await _controller.GetMenuItem(existingMenuItemId);
        var okResult = result as OkObjectResult;
        var menuItemDTO = okResult.Value as MenuItemDTO;

        Assert.That(menuItemDTO, Has.Property("Name").EqualTo("Item 1"));
    }

    [Test]
    public async Task GetMenuItem_ReturnsCorrectPrice()
    {
        int existingMenuItemId = 1;

        var result = await _controller.GetMenuItem(existingMenuItemId);
        var okResult = result as OkObjectResult;
        var menuItemDTO = okResult.Value as MenuItemDTO;

        Assert.That(menuItemDTO, Has.Property("Price").EqualTo(100));
    }

    [Test]
    public async Task GetMenuItem_ReturnsCorrectCategory()
    {
        int existingMenuItemId = 1;

        var result = await _controller.GetMenuItem(existingMenuItemId);
        var okResult = result as OkObjectResult;
        var menuItemDTO = okResult.Value as MenuItemDTO;

        Assert.That(menuItemDTO, Has.Property("Category").EqualTo("Category 1"));
    }

    [Test]
    public async Task GetMenuItem_ReturnsCorrectHasAllergens()
    {
        int existingMenuItemId = 1;

        var result = await _controller.GetMenuItem(existingMenuItemId);
        var okResult = result as OkObjectResult;
        var menuItemDTO = okResult.Value as MenuItemDTO;

        Assert.That(menuItemDTO, Has.Property("HasAllergens").EqualTo(false));
    }

    [Test]
    public async Task GetMenuItem_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingMenuItemId = 999;

        // Act
        var result = await _controller.GetMenuItem(nonExistingMenuItemId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Item with given ID doesn't exist"));
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
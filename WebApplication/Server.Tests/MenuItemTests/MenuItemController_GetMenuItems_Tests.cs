using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MenuItemTests;

[TestFixture]
public class MenuItemController_GetMenuItems_Tests
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
        _context.MenuItems.Add(new MenuItem { 
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
    public async Task GetMenuItems_ReturnsCorrectNumberOfItems()
    {
        var result = await _controller.GetMenuItems();
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task GetMenuItems_ReturnsCorrectName([Values(0, 1, 2)] int index)
    {
        var result = await _controller.GetMenuItems();
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems[index], Has.Property("Name").EqualTo($"Item {index+1}"));
    }

    [Test]
    public async Task GetMenuItems_ReturnsCorrectPrice([Values(0, 1, 2)] int index)
    {
        var result = await _controller.GetMenuItems();
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems[index], Has.Property("Price").EqualTo(100 * (index+1)));
    }

    [Test]
    public async Task GetMenuItems_ReturnsCorrectHasAllergens([Values(0, 1, 2)] int index)
    {
        // Arrange
        var expectedResults = new List<bool> { false, true, true };

        var result = await _controller.GetMenuItems();
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems[index], Has.Property("HasAllergens").EqualTo(expectedResults[index]));
    }

    [Test]
    public async Task GetMenuItems_ReturnsCorrectCategory([Values(0, 1, 2)] int index)
    {
        var result = await _controller.GetMenuItems();
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems[index], Has.Property("Category").EqualTo($"Category {index+1}"));
    }

    [Test]
    public async Task GetMenuItems_ReturnsEmptyList()
    {
        // Arrange
        _context.MenuItems.RemoveRange(_context.MenuItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetMenuItems();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems, Is.Empty);
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
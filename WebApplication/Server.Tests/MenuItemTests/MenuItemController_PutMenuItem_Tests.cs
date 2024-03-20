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
public class MenuItemController_PutMenuItem_Tests
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
    public async Task PutMenuItem_WithValidData_ReturnsNoContentResult()
    {
        var existingMenuItemId = 1; // Ensure this matches a seeded MenuItem
        var updateDTO = new MenuItemDTO { MenuItemID = existingMenuItemId, Name = "Updated Name", Price = 1200, Category = Category.Chinese.ToString(), HasAllergens = true };

        var result = await _controller.PutMenuItem(existingMenuItemId, updateDTO);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task PutMenuItem_UpdatesNamePropertyCorrectly()
    {
        var existingMenuItemId = 1;
        var updateDTO = new MenuItemDTO { MenuItemID = existingMenuItemId, Name = "Updated Name", Price = 1200, Category = Category.Chinese.ToString(), HasAllergens = true };

        await _controller.PutMenuItem(existingMenuItemId, updateDTO);

        var updatedItem = await _context.MenuItems.FindAsync(existingMenuItemId);
        Assert.That(updatedItem.Name, Is.EqualTo(updateDTO.Name));
    }

    [Test]
    public async Task PutMenuItem_UpdatesPricePropertyCorrectly()
    {
        var existingMenuItemId = 1;
        var updateDTO = new MenuItemDTO { MenuItemID = existingMenuItemId, Name = "Updated Item", Price = 1200, Category = Category.Chinese.ToString(), HasAllergens = true };

        await _controller.PutMenuItem(existingMenuItemId, updateDTO);

        var updatedItem = await _context.MenuItems.FindAsync(existingMenuItemId);
        Assert.That(updatedItem.Price, Is.EqualTo(updateDTO.Price));
    }

    [Test]
    public async Task PutMenuItem_UpdatesCategoryPropertyCorrectly()
    {
        var existingMenuItemId = 1;
        var updateDTO = new MenuItemDTO { MenuItemID = existingMenuItemId, Name = "Updated Item", Price = 1200, Category = Category.Chinese.ToString(), HasAllergens = true };

        await _controller.PutMenuItem(existingMenuItemId, updateDTO);

        var updatedItem = await _context.MenuItems.FindAsync(existingMenuItemId);
        Assert.That(updatedItem.Category, Is.EqualTo(updateDTO.Category));
    }

    [Test]
    public async Task PutMenuItem_UpdatesHasAllergensPropertyCorrectly()
    {
        var existingMenuItemId = 1;
        var updateDTO = new MenuItemDTO { MenuItemID = existingMenuItemId, Name = "Updated Item", Price = 1200, Category = Category.Chinese.ToString(), HasAllergens = true };

        await _controller.PutMenuItem(existingMenuItemId, updateDTO);

        var updatedItem = await _context.MenuItems.FindAsync(existingMenuItemId);
        Assert.That(updatedItem.HasAllergens, Is.EqualTo(updateDTO.HasAllergens));
    }

    [Test]
    public async Task PutMenuItem_WithNameTooLong_ReturnsBadRequest()
    {
        var menuItemDTO = new MenuItemDTO { MenuItemID = 1, Name = new string('a', 81), Price = 100 };
        var result = await _controller.PutMenuItem(1, menuItemDTO);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Name can't have more than 80 character"));
    }

    [Test]
    public async Task PutMenuItem_WithNegativePrice_ReturnsBadRequest()
    {
        var menuItemDTO = new MenuItemDTO { MenuItemID = 1, Name = "Test Item", Price = -1 };
        var result = await _controller.PutMenuItem(1, menuItemDTO);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Price can't be negative"));
    }

    [Test]
    public async Task PutMenuItem_WithMismatchedIds_ReturnsBadRequest()
    {
        var menuItemDTO = new MenuItemDTO { MenuItemID = 2, Name = "Test Item", Price = 100 };
        var result = await _controller.PutMenuItem(1, menuItemDTO);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("Item IDs don't match"));
    }

    [Test]
    public async Task PutMenuItem_WithInvalidCategory_ReturnsBadRequest()
    {
        var menuItemDTO = new MenuItemDTO { MenuItemID = 1, Name = "Test Item", Price = 100, Category = "InvalidCategory" };
        var result = await _controller.PutMenuItem(1, menuItemDTO);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Has.Property("Value").EqualTo("The given category doesn't exist"));
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
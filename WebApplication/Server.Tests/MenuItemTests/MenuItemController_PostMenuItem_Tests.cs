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
public class MenuItemController_PostMenuItem_Tests
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

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task PostMenuItem_WithValidData_ReturnsCreatedAtActionResult()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = Category.Indian.ToString(), HasAllergens = false };

        var result = await _controller.PostMenuItem(newItem);

        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task PostMenuItem_WithValidData_SetsCorrectName()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = Category.Indian.ToString(), HasAllergens = false };

        var result = await _controller.PostMenuItem(newItem);
        var createdResult = result as CreatedAtActionResult;
        var createdItem = createdResult.Value as MenuItemDTO;

        Assert.That(createdItem, Has.Property("Name").EqualTo(newItem.Name));
    }

    [Test]
    public async Task PostMenuItem_WithValidData_SetsCorrectPrice()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = Category.Indian.ToString(), HasAllergens = false };

        var result = await _controller.PostMenuItem(newItem);
        var createdResult = result as CreatedAtActionResult;
        var createdItem = createdResult.Value as MenuItemDTO;

        Assert.That(createdItem, Has.Property("Price").EqualTo(newItem.Price));
    }

    [Test]
    public async Task PostMenuItem_WithValidData_SetsCorrectCategory()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = Category.Indian.ToString(), HasAllergens = false };

        var result = await _controller.PostMenuItem(newItem);
        var createdResult = result as CreatedAtActionResult;
        var createdItem = createdResult.Value as MenuItemDTO;

        Assert.That(createdItem, Has.Property("Category").EqualTo(newItem.Category));
    }

    [Test]
    public async Task PostMenuItem_WithValidData_SetsHasAllergensCorrectly()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = Category.Indian.ToString(), HasAllergens = false };

        var result = await _controller.PostMenuItem(newItem);
        var createdResult = result as CreatedAtActionResult;
        var createdItem = createdResult.Value as MenuItemDTO;

        Assert.That(createdItem, Has.Property("HasAllergens").EqualTo(newItem.HasAllergens));
    }

    [Test]
    public async Task PostMenuItem_WithInvalidModelState_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("error", "model state is invalid");

        var newItem = new MenuItemDTO();

        var result = await _controller.PostMenuItem(newItem);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PostMenuItem_WithNameTooLong_ReturnsBadRequest()
    {
        var newItem = new MenuItemDTO { Name = new string('a', 81), Price = 100, Category = Category.Italian.ToString() }; // 81 characters

        var result = await _controller.PostMenuItem(newItem);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("Name can't have more than 80 character"));
    }

    [Test]
    public async Task PostMenuItem_WithNegativePrice_ReturnsBadRequest()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = -1, Category = Category.Mexican.ToString() };

        var result = await _controller.PostMenuItem(newItem);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("Price can't be negative"));
    }

    [Test]
    public async Task PostMenuItem_WithInvalidCategory_ReturnsBadRequest()
    {
        var newItem = new MenuItemDTO { Name = "New Item", Price = 100, Category = "NonExistingCategory" };

        var result = await _controller.PostMenuItem(newItem);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("The given category doesn't exist"));
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
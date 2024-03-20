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
public class MenuItemController_GetMenuCategories_Tests
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
    public async Task GetMenuCategories_ReturnsAllCategories()
    {
        // Act
        var result = await _controller.GetMenuCategories();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var categories = okResult.Value as List<string>;

        var expectedCategories = Enum.GetNames(typeof(Category)).ToList();

        Assert.That(categories, Has.Count.EqualTo(expectedCategories.Count));

        foreach (var expectedCategory in expectedCategories)
        {
            Assert.That(categories, Does.Contain(expectedCategory));
        }
    }

    [Test]
    public async Task GetMenuCategories_ReturnsCategoriesInDefinedOrder()
    {
        // Arrange
        var expectedCategories = Enum.GetNames(typeof(Category)).ToList();

        // Act
        var result = await _controller.GetMenuCategories();
        var okResult = result as OkObjectResult;
        var categories = okResult.Value as List<string>;

        // Assert
        for (int i = 0; i < expectedCategories.Count; i++)
        {
            Assert.That(categories[i], Is.EqualTo(expectedCategories[i]));
        }
    }

    [Test]
    public async Task GetMenuCategories_ContainsSpecificCategory([ValueSource(nameof(CategoryValues))] string categoryName)
    {
        // Act
        var result = await _controller.GetMenuCategories();
        var okResult = result as OkObjectResult;
        var categories = okResult.Value as List<string>;

        // Assert
        Assert.That(categories, Does.Contain(categoryName));
    }

    [TearDown]
    public void TearDown()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    public static IEnumerable<string> CategoryValues()
    {
        return Enum.GetNames(typeof(Category));
    }
}
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
public class MenuItemController_GetMenuItemsForOrder_Tests
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
            HasAllergies = false,
            HasDiscount = true,
            Money = 2200,
            Name = "Guest 2",
            TableID = 2
        });
        _context.SaveChanges();
        _context.Guests.Add(new Guest
        {
            GuestID = 3,
            HasAllergies = true,
            HasDiscount = false,
            Money = 3200,
            Name = "Guest 3",
            TableID = 3
        });
        _context.SaveChanges();

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

        _context.Orders.Add(new Order
        {
            OrderID = 1,
            GuestID = 1,
            MenuItemID = 1,
            Quantity = 1
        });
        _context.Orders.Add(new Order
        {
            OrderID = 2,
            GuestID = 2,
            MenuItemID = 2,
            Quantity = 1
        });
        _context.SaveChanges();

        // Setup the transaction just in case
        _transaction = _context.Database.BeginTransaction();
    }

    [Test]
    public async Task GetMenuItemsForOrder_WithNonExistingGuest_ReturnsNotFound()
    {
        int nonExistingGuestId = 999;

        var result = await _controller.GetMenuItemsForOrder(nonExistingGuestId);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Has.Property("Value").EqualTo("Guest with given ID doesn't exist"));
    }

    [Test]
    public async Task GetMenuItemsForOrder_ExistingGuestNoAllergiesNoDiscount_ReturnsFullPriceItems([Values(0, 1, 2)] int index)
    {
        int existingGuestId = 1;

        var result = await _controller.GetMenuItemsForOrder(existingGuestId);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems, Is.Not.Empty);

        Assert.That(menuItems, Has.Count.EqualTo(3));

        var item = menuItems[index];
        var menuItem = _context.MenuItems.FirstOrDefault(mi => mi.MenuItemID == item.MenuItemID);
        Assert.That(item, Has.Property("Price").EqualTo(menuItem.Price));
    }

    [Test]
    public async Task GetMenuItemsForOrder_ExistingGuestWithAllergies_FiltersAllergens()
    {
        int guestWithAllergiesId = 3;

        var result = await _controller.GetMenuItemsForOrder(guestWithAllergiesId);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems, Is.Not.Empty);

        Assert.That(menuItems, Has.Count.EqualTo(1));

        Assert.That(menuItems[0], Has.Property("HasAllergens").False);
    }

    [Test]
    public async Task GetMenuItemsForOrder_ExistingGuestWithDiscount_AppliesDiscountToPrices([Values(0, 1, 2)] int index)
    {
        int guestWithDiscountId = 2;

        var result = await _controller.GetMenuItemsForOrder(guestWithDiscountId);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var menuItems = okResult.Value as List<MenuItemDTO>;
        Assert.That(menuItems, Is.Not.Empty);
       
        Assert.That(menuItems, Has.Count.EqualTo(3));

        var item = menuItems[index];
        var menuItem = _context.MenuItems.FirstOrDefault(mi => mi.MenuItemID == item.MenuItemID);

        var discountFactor = 0.85;
        var expectedResult = (int)(menuItem.Price * discountFactor);
        Assert.That(item, Has.Property("Price").EqualTo(expectedResult));
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
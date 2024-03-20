using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Server.Controllers;
using AutoMapper;
using PlaywrightTests;


namespace Playwright.Codegen;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PubTests : PageTest
{
    private static PubContext _context;

    [SetUp]
    public async Task SetUp()
    {
        var optionsBuilder = new DbContextOptionsBuilder<PubContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\PubDatabase;Database=Pub");

        var options = optionsBuilder.Options;
        _context = new PubContext(options);

        // Refresh the database
        await DatabaseRefresher.AddDataAsync(_context);
    }

    [Test]
    public async Task AddTable()
    {
        await Page.GotoAsync("http://localhost:3000/tables");
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Table" }).ClickAsync();
        await Page.Locator("input[name=\"number\"]").ClickAsync();
        await Page.Locator("input[name=\"number\"]").FillAsync("109");
        await Page.Locator("input[name=\"number\"]").ClickAsync();
        await Page.Locator("input[name=\"seats\"]").FillAsync("6");
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "Available" });
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Table" }).ClickAsync();
    }

    [Test]
    public async Task EditTable()
    {
        await Page.GotoAsync("http://localhost:3000/tables");
        await Page.Locator("#edit_1").ClickAsync();
        await Page.Locator("input[name=\"number\"]").ClickAsync();
        await Page.Locator("input[name=\"number\"]").FillAsync("100");
        await Page.Locator("input[name=\"seats\"]").ClickAsync();
        await Page.Locator("input[name=\"seats\"]").FillAsync("6");
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "Occupied" });
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Edit Table" }).ClickAsync();
    }

    [Test]
    public async Task InfoTable()
    {
        await Page.GotoAsync("http://localhost:3000/tables");
        await Page.Locator("#info_2").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Edit" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Tables" }).ClickAsync();
        await Page.Locator("#info_2").ClickAsync();
        await Page.Locator("#guest_1").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Tables" }).ClickAsync();
        await Page.Locator("#info_2").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Delete" }).ClickAsync();
    }

    [Test]
    public async Task DeleteTable()
    {
        await Page.GotoAsync("http://localhost:3000/tables");
        await Page.Locator("#delete_1").ClickAsync();
        await Page.Locator("#delete_3").ClickAsync();
        await Page.Locator("#delete_5").ClickAsync();
        await Page.Locator("#delete_7").ClickAsync();
    }

    [Test]
    public async Task AddGuest()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Guest" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Milos");
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("240000");
        await Page.Locator("input[name=\"hasDiscount\"]").CheckAsync();
        await Page.Locator("input[name=\"hasAllergies\"]").CheckAsync();
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "108" });
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Guest" }).ClickAsync();
    }

    [Test]
    public async Task EditGuest()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.Locator("#edit_5").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Enislava");
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("3800");
        await Page.Locator("input[name=\"hasDiscount\"]").CheckAsync();
        await Page.Locator("input[name=\"hasAllergies\"]").UncheckAsync();
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "105" });
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Edit Guest" }).ClickAsync();
    }

    [Test]
    public async Task InfoGuest()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.Locator("#info_4").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Edit" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Guests" }).ClickAsync();
        await Page.Locator("#info_4").ClickAsync();
        await Page.Locator("#cancel_5").ClickAsync();
        await Page.GetByText("Delete").ClickAsync();
    }

    [Test]
    public async Task DeleteGuest()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.Locator("#delete_1").ClickAsync();
        await Page.Locator("#delete_4").ClickAsync();
        await Page.Locator("#delete_7").ClickAsync();
        await Page.Locator("#delete_10").ClickAsync();
    }

    [Test]
    public async Task PayOrder()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.Locator("#info_5").ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("300");
        await Page.GetByText("Pay").ClickAsync();
    }

    [Test]
    public async Task MakeOrder()
    {
        await Page.GotoAsync("http://localhost:3000/guests");
        await Page.Locator("#make_order_1").ClickAsync();
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "1" });
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("3");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Guests" }).ClickAsync();
        await Page.Locator("#make_order_2").ClickAsync();
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "5" });
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("2");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Order" }).ClickAsync();
    }

    [Test]
    public async Task AddMenuOrder()
    {
        await Page.GotoAsync("http://localhost:3000/menu");
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Menu Item" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Test item");
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("360");
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "International" });
        await Page.GetByRole(AriaRole.Checkbox).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Menu Item" }).ClickAsync();
    }

    [Test]
    public async Task EditMenuOrder()
    {
        await Page.GotoAsync("http://localhost:3000/menu");
        await Page.Locator("#edit_1").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Burger New");
        await Page.GetByRole(AriaRole.Spinbutton).ClickAsync();
        await Page.GetByRole(AriaRole.Spinbutton).FillAsync("390");
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "Chinese" });
        await Page.GetByRole(AriaRole.Checkbox).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Edit Item" }).ClickAsync();
        await Page.Locator("#edit_3").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Snails New");
        await Page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "Mexican" });
        await Page.GetByRole(AriaRole.Checkbox).UncheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Edit Item" }).ClickAsync();
    }

    [Test]
    public async Task DeleteMenuOrder()
    {
        await Page.GotoAsync("http://localhost:3000/menu");
        await Page.Locator("#delete_1").ClickAsync();
        await Page.Locator("#delete_3").ClickAsync();
        await Page.Locator("#delete_6").ClickAsync();
        await Page.Locator("#delete_8").ClickAsync();
    }

    [Test]
    public async Task AddWaiter()
    {
        await Page.GotoAsync("http://localhost:3000/waiters");
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Waiter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Milos");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Waiter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Waiter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Dejan");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Waiter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Add Waiter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox).FillAsync("Darko");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Add Waiter" }).ClickAsync();
    }

    [Test]
    public async Task ViewOrdersWaiter()
    {
        await Page.GotoAsync("http://localhost:3000/waiters");
        await Page.Locator("#view_orders_1").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Waiters" }).ClickAsync();
        await Page.Locator("#view_orders_3").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { NameString = "Waiters" }).ClickAsync();
        await Page.Locator("#view_orders_5").ClickAsync();
    }

    [Test]
    public async Task DeleteWaiter()
    {
        await Page.GotoAsync("http://localhost:3000/waiters");
        await Page.Locator("#delete_1").ClickAsync();
        await Page.Locator("#delete_3").ClickAsync();
        await Page.Locator("#delete_6").ClickAsync();
    }

    [Test]
    public async Task DeliverOrderWaiter()
    {
        await Page.GotoAsync("http://localhost:3000/waiters");
        await Page.Locator("#view_orders_2").ClickAsync();

        // simulate page refreshing to make sure there are completed orders
        for (int i = 0; i < 20; i++)
        {
            await Page.GotoAsync("http://localhost:3000/waiters/orders/2");
        }

        await Page.Locator("#deliver_1").ClickAsync();
        await Page.Locator("#deliver_4").ClickAsync();
        await Page.Locator("#deliver_7").ClickAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }
}

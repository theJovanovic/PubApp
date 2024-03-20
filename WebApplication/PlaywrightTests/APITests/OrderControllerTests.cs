
using Microsoft.EntityFrameworkCore;
using Models;

namespace PlaywrightTests.APITests
{
    public class OrderControllerTests : PlaywrightTest
    {
        private IAPIRequestContext Request;
        private PubContext _context;

        [SetUp]
        public async Task SetUpAPITesting()
        {
            var headers = new Dictionary<string, string>
            {
                {"Accept", "applicaiton/json"},
            };

            Request = await Playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = "https://localhost:7146",
                ExtraHTTPHeaders = headers,
                IgnoreHTTPSErrors = true
            });

            var optionsBuilder = new DbContextOptionsBuilder<PubContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\PubDatabase;Database=Pub");

            var options = optionsBuilder.Options;
            _context = new PubContext(options);

            // Refresh the database
            await DatabaseRefresher.AddDataAsync(_context);
        }

        [Test]
        public async Task PostOrder_ValidData_ReturnsCreatedAtAction()
        {
            int newOrderId = 11;

            await using var response = await Request.PostAsync("/api/Order", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Quantity = 2,
                    MenuItemID = 3
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("orderID", out var orderID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(201));
                Assert.That((int)orderID, Is.EqualTo(newOrderId));
            });
        }

        [Test]
        public async Task PostOrder_NegativeQuantity_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Order", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Quantity = -2,
                    MenuItemID = 3
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Quantity must be a positive value"));
            });
        }

        [Test]
        public async Task DeleteOrder_ExistingOrderId_ReturnsNoContent()
        {
            int deleteOrdertId = 1;

            await using var response = await Request.DeleteAsync($"/api/Order/{deleteOrdertId}");

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeleteOrder_NonExistingOrderId_ReturnsNotFound()
        {
            int deleteOrdertId = 999;

            await using var response = await Request.DeleteAsync($"/api/Order/{deleteOrdertId}");

            Assert.That(response, Has.Property("Status").EqualTo(404));
        }

        [TearDown]
        public async Task TearDownAPITesting()
        {
            await Request.DisposeAsync();
            await _context.DisposeAsync();
        }
    }
}

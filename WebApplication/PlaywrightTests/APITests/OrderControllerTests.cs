
using Microsoft.EntityFrameworkCore;
using Models;

namespace PlaywrightTests.APITests
{
    [TestFixture]
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

        [Test]
        public async Task PayOrder_ValidData_ReturnsNoContent()
        {
            int orderId = 8;
            int tip = 100;

            await using var response = await Request.DeleteAsync($"/api/Order/pay/{orderId}/{tip}");

            await using var response1 = await Request.GetAsync($"/api/Order/{orderId}");

            var body = await response1.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response1, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Order with given ID doesn't exist"));
                Assert.That(response, Has.Property("Status").EqualTo(204));
            });
        }

        [Test]
        public async Task PayOrder_NegativeTips_ReturnsBadRequest()
        {
            int orderId = 8;
            int tip = -100;

            await using var response = await Request.DeleteAsync($"/api/Order/pay/{orderId}/{tip}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(jsonString, Does.Contain("Tip can't be a negative value"));
                Assert.That(response, Has.Property("Status").EqualTo(400));
            });
        }

        [Test]
        public async Task PayOrder_NonExistingOrderId_ReturnsNotFound()
        {
            int orderId = -8;
            int tip = 100;

            await using var response = await Request.DeleteAsync($"/api/Order/pay/{orderId}/{tip}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(jsonString, Does.Contain("Order with given ID doesn't exist"));
                Assert.That(response, Has.Property("Status").EqualTo(404));
            });
        }

        [Test]
        public async Task PayOrder_GuestNotEnoughMoney_ReturnsBadRequest()
        {
            int orderId = 10;
            int tip = 100;

            await using var response = await Request.DeleteAsync($"/api/Order/pay/{orderId}/{tip}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(jsonString, Does.Contain("Guest doesn't have enough money to pay the order"));
                Assert.That(response, Has.Property("Status").EqualTo(400));
            });
        }

        [Test]
        public async Task DeliverOrder_ValidData_ReturnsNoContent()
        {
            int orderId = 5;
            int waiterId = 1;

            await using var response = await Request.PutAsync($"/api/Order/{orderId}/Waiter/{waiterId}");
            
            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeliverOrder_NonExistingOrderId_ReturnsNotFound()
        {
            int orderId = 99;
            int waiterId = 1;

            await using var response = await Request.PutAsync($"/api/Order/{orderId}/Waiter/{waiterId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Order with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task DeliverOrder_NonExistingWaiterId_ReturnsNotFound()
        {
            int orderId = 5;
            int waiterId = 99;

            await using var response = await Request.PutAsync($"/api/Order/{orderId}/Waiter/{waiterId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Waiter with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task DeliverOrder_NotCompletedOrder_ReturnsBadRequest()
        {
            int orderId = 1;
            int waiterId = 2;

            await using var response = await Request.PutAsync($"/api/Order/{orderId}/Waiter/{waiterId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Order is not completed"));
            });
        }

        [Test]
        public async Task GetOrders_NonEmptyOrders_ReturnsOk()
        {
            await using var response = await Request.GetAsync("/api/Order");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(result, Has.Count.GreaterThan(0));
            });
        }

        [Test]
        public async Task GetOrder_ExistingOrderId_ReturnsOkAndOrder()
        {
            int orderId = 1;

            await using var response = await Request.GetAsync($"/api/Order/{orderId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("orderID", out var orderID);
            result.AsObject().TryGetPropertyValue("orderTime", out var orderTime);
            result.AsObject().TryGetPropertyValue("status", out var status);
            result.AsObject().TryGetPropertyValue("guestID", out var guestID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)orderID, Is.EqualTo(orderId));
                Assert.That((DateTime)orderTime, Is.EqualTo(new DateTime(2024,03,19,12,30,00)));
                Assert.That((string)status, Is.EqualTo("Preparing"));
                Assert.That((int)guestID, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetOrder_NonExistingOrderId_ReturnsNotFound()
        {
            int orderId = -999;

            await using var response = await Request.GetAsync($"/api/Order/{orderId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Order with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task GetOrdersOverview_NonEmptyOrders_ReturnsOk()
        {
            await using var response = await Request.GetAsync("/api/Order/overview");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(result, Has.Count.GreaterThan(0));
            });
        }

        [TearDown]
        public async Task TearDownAPITesting()
        {
            await Request.DisposeAsync();
            await _context.DisposeAsync();
        }
    }
}

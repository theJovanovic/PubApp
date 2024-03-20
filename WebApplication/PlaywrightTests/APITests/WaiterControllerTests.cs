using Microsoft.EntityFrameworkCore;
using Models;

namespace PlaywrightTests.APITests
{
    public class WaiterControllerTests : PlaywrightTest
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
        public async Task PostWaiter_ValidData_ReturnsCreatedAtAction()
        {
            int newWaiterId = 7;

            await using var response = await Request.PostAsync("/api/Waiter", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Tips = 0,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("waiterID", out var waiterID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(201));
                Assert.That((int)waiterID, Is.EqualTo(newWaiterId));
            });
        }

        [Test]
        public async Task PostWaiter_TooLongName_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Waiter", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "THIS IS A VEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERY LONG NAMEEEEE WAITER 😴",
                    Tips = 0
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Name can't have more than 50 characters"));
            });
        }

        [Test]
        public async Task PostWaiter_SetTips_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Waiter", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Tips = 1
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Tips can't be set"));
            });
        }

        [Test]
        public async Task DeleteWaiter_ExistingWaiterId_ReturnsNoContent()
        {
            int deleteWaiterId = 6;

            await using var response = await Request.DeleteAsync($"/api/Waiter/{deleteWaiterId}");

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeleteWaiter_NonExistingWaiterId_ReturnsNotFound()
        {
            int deleteWaiterId = 999;

            await using var response = await Request.DeleteAsync($"/api/Waiter/{deleteWaiterId}");

            Assert.That(response, Has.Property("Status").EqualTo(404));
        }

        [Test]
        public async Task PutWaiter_ValidData_ReturnsNoContent()
        {
            int putWaiterId = 6;

            await using var response = await Request.PutAsync($"/api/Waiter/{putWaiterId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    WaiterID = 6,
                    Name = "TestName",
                    Tips = 0,
                }
            });

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task PutWaiter_TooLongName_ReturnsBadRequest()
        {
            int putWaiterId = 6;

            await using var response = await Request.PutAsync($"/api/Waiter/{putWaiterId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    WaiterID = 6,
                    Name = "VEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERY LOOOOOOOOOOOOOOOOOOOOOOOONG NAME",
                    Tips = 999
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Name can't have more than 50 characters"));
            });
        }

        [Test]
        public async Task PutWaiter_NegativeTips_ReturnsBadRequest()
        {
            int putWaiterId = 6;

            await using var response = await Request.PutAsync($"/api/Waiter/{putWaiterId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    WaiterID = 6,
                    Name = "TestName",
                    Tips = -999
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Tips can't be negative"));
            });
        }

        [Test]
        public async Task PutWaiter_WaiterIdsDontMatch_ReturnsBadRequest()
        {
            int putWaiterId = 6;

            await using var response = await Request.PutAsync($"/api/Waiter/{putWaiterId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    WaiterID = 5,
                    Name = "TestName",
                    Tips = 999
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Waiter IDs don't match"));
            });
        }

        [Test]
        public async Task PutWaiter_NonExistingWaiterId_ReturnsNotFound()
        {
            int putWaiterId = 999;

            await using var response = await Request.PutAsync($"/api/Waiter/{putWaiterId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    WaiterID = 999,
                    Name = "TestName",
                    Tips = 999
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Waiter with given ID doesn't exist"));
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

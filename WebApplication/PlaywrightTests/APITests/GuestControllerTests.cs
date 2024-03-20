using Microsoft.EntityFrameworkCore;
using Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PlaywrightTests.APITests
{
    [TestFixture]
    public class GuestControllerTests : PlaywrightTest
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
        public async Task PostGuest_ValidData_ReturnsCreatedAtAction()
        {
            int newGuestId = 12;

            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = 106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("guestID", out var guestID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(201));
                Assert.That((int)guestID, Is.EqualTo(newGuestId));
            });
        }

        [Test]
        public async Task PostGuest_TooLongName_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "THIS IS A VEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERY LONG NAMEEEEE 😁",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = 106
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
        public async Task PostGuest_NegativeMoney_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Money = -9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = 106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Money can't be negative"));
            });
        }

        [Test]
        public async Task PostGuest_NegativeTableNumber_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = -106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table number must be positive value"));
            });
        }

        [Test]
        public async Task PostGuest_NonExistingTableNumber_ReturnsNotFound()
        {
            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = 999
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Table with given number doesn't exist"));
            });
        }

        [Test]
        public async Task PostGuest_AlreadyFullTable_ReturnsConflict()
        {
            await using var response = await Request.PostAsync("/api/Guest", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = false,
                    TableNumber = 107
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(409));
                Assert.That(jsonString, Does.Contain("Table is already full"));
            });
        }

        [Test]
        public async Task DeleteGuest_ExistingGuestId_ReturnsNoContent()
        {
            int deleteGuestId = 1;

            await using var response = await Request.DeleteAsync($"/api/Guest/{deleteGuestId}");

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeleteGuest_NonExistingGuestId_ReturnsNotFound()
        {
            int deleteGuestId = 999;

            await using var response = await Request.DeleteAsync($"/api/Guest/{deleteGuestId}");

            Assert.That(response, Has.Property("Status").EqualTo(404));
        }

        [Test]
        public async Task PutGuest_ValidData_ReturnsNoContent()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 106
                }
            });

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task PutGuest_TooLongName_ReturnsBadRequest()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "VEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERY LOOOOOOOOOOOOOOOOOOOOOOOONG NAME",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 106
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
        public async Task PutGuest_NegativeMoney_ReturnsBadRequest()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "TestName",
                    Money = -9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Money can't be negative"));
            });
        }

        [Test]
        public async Task PutGuest_NegativeTableNumber_ReturnsBadRequest()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = -106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table number must be positive value"));
            });
        }

        [Test]
        public async Task PutGuest_AlreadyFullTable_ReturnsBadRequest()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 107
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table is already full"));
            });
        }

        [Test]
        public async Task PutGuest_GuestIdsDontMatch_ReturnsBadRequest()
        {
            int putGuestId = 2;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 1,
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Guest IDs don't match"));
            });
        }

        [Test]
        public async Task PutGuest_NonExistingGuestId_ReturnsNotFound()
        {
            int putGuestId = 999;

            await using var response = await Request.PutAsync($"/api/Guest/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    GuestID = 999,
                    Name = "TestName",
                    Money = 9999,
                    HasAllergies = true,
                    HasDiscount = true,
                    TableNumber = 106
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Guest with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task GetGuests_NonEmptyGuests_ReturnsOk()
        {
            await using var response = await Request.GetAsync("/api/Guest");

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
        public async Task GetGuest_ExistingGuestId_ReturnsOkAndGuest()
        {
            int guestId = 1;

            await using var response = await Request.GetAsync($"/api/Guest/{guestId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("guestID", out var guestID);
            result.AsObject().TryGetPropertyValue("name", out var name);
            result.AsObject().TryGetPropertyValue("money", out var money);
            result.AsObject().TryGetPropertyValue("hasAllergies", out var hasAllergies);
            result.AsObject().TryGetPropertyValue("hasDiscount", out var hasDiscount);
            result.AsObject().TryGetPropertyValue("tableNumber", out var tableNumber);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)guestID, Is.EqualTo(guestId));
                Assert.That((string)name, Is.EqualTo("Dusan"));
                Assert.That((int)money, Is.EqualTo(1000));
                Assert.That((bool)hasAllergies, Is.False);
                Assert.That((bool)hasDiscount, Is.False);
                Assert.That((int)tableNumber, Is.EqualTo(102));
            });
        }

        [Test]
        public async Task GetGuest_NonExistingGuestId_ReturnsNotFound()
        {
            int guestId = -999;

            await using var response = await Request.GetAsync($"/api/Guest/{guestId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Guest with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task GetGuestInfo_ExistingGuestId_ReturnsOkAndGuestInfo()
        {
            int guestId = 1;

            await using var response = await Request.GetAsync($"/api/Guest/info/{guestId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("guestID", out var guestID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)guestID, Is.EqualTo(guestId));
            });
        }

        [Test]
        public async Task GetGuestInfo_NonExistingGuestId_ReturnsNotFound()
        {
            int guestId = -999;

            await using var response = await Request.GetAsync($"/api/Guest/info/{guestId}");
            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Guest with given ID doesn't exist"));
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

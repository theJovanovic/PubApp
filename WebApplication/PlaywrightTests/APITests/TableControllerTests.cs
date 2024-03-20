using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Models;

namespace PlaywrightTests.APITests
{
    public class TableControllerTests : PlaywrightTest
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
        public async Task PostTable_ValidData_ReturnsCreatedAtAction()
        {
            int newTableId = 9;

            await using var response = await Request.PostAsync("/api/Table", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Number = 109,
                    Seats = 3,
                    Status = "Available"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("tableID", out var tableID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(201));
                Assert.That((int)tableID, Is.EqualTo(newTableId));
            });
        }

        [Test]
        public async Task PostTable_NegativeTableNumber_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Table", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Number = -109,
                    Seats = 3,
                    Status = "Available"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table number must be a positive value"));
            });
        }

        [Test]
        public async Task PostTable_NegativeSeats_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/Table", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Number = 109,
                    Seats = -3,
                    Status = "Available"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);


            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Seats must be a positive value"));
            });
        }

        [Test]
        public async Task PostTable_TableNumberTaken_ReturnsConflict()
        {
            await using var response = await Request.PostAsync("/api/Table", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Number = 101,
                    Seats = 3,
                    Status = "Available"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(409));
                Assert.That(jsonString, Does.Contain("Table with the same number already exist"));
            });
        }

        [Test]
        public async Task DeleteTable_ExistingTableId_ReturnsNoContent()
        {
            int tableId = 1;

            await using var response = await Request.DeleteAsync($"/api/Table/{tableId}");

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeleteTable_NonExistingTableId_ReturnsNotFound()
        {
            int tableId = 999;

            await using var response = await Request.DeleteAsync($"/api/Table/{tableId}");

            Assert.That(response, Has.Property("Status").EqualTo(404));
        }

        [Test]
        public async Task PutTable_ValidData_ReturnsNoContent()
        {
            int tableId = 3;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 3,
                    Number = 999,
                    Seats = 99,
                    Status = "Occupied"
                }
            });

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task PutTable_NegativeTableNumber_ReturnsBadRequest()
        {
            int tableId = 3;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 3,
                    Number = -999,
                    Seats = 99,
                    Status = "Occupied"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table number must be a positive value"));
            });
        }

        [Test]
        public async Task PutTable_NegativeSeats_ReturnsBadRequest()
        {
            int tableId = 3;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 3,
                    Number = 999,
                    Seats = -99,
                    Status = "Occupied"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Seats must be a positive value"));
            });
        }

        [Test]
        public async Task PutTable_TableIdsDontMatch_ReturnsBadRequest()
        {
            int tableId = 2;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 3,
                    Number = 101,
                    Seats = 99,
                    Status = "Occupied"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Table IDs doesn't match"));
            });
        }

        [Test]
        public async Task PutTable_NonExistingTableId_ReturnsNotFound()
        {
            int tableId = 999;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 999,
                    Number = 101,
                    Seats = 99,
                    Status = "Occupied"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Table with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task PutTable_TableNumberTaken_ReturnsConflict()
        {
            int tableId = 3;

            await using var response = await Request.PutAsync($"/api/Table/{tableId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    TableID = 3,
                    Number = 101,
                    Seats = 99,
                    Status = "Occupied"
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(409));
                Assert.That(jsonString, Does.Contain("Table with the same number already exist"));
            });
        }

        [Test]
        public async Task GetTables_ReturnsOk_AndNonEmptyTables()
        {
            await using var response = await Request.GetAsync("/api/Table");

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
        public async Task GetTable_ExistingTableId_ReturnsOkAndTable()
        {
            int tableId = 1;

            await using var response = await Request.GetAsync($"/api/Table/{tableId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("tableID", out var tableID);
            result.AsObject().TryGetPropertyValue("number", out var number);
            result.AsObject().TryGetPropertyValue("seats", out var seats);
            result.AsObject().TryGetPropertyValue("status", out var status);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)tableID, Is.EqualTo(tableId));
                Assert.That((int)number, Is.EqualTo(101));
                Assert.That((int)seats, Is.EqualTo(5));
                Assert.That((string)status, Is.EqualTo("Available"));
            });
        }

        [Test]
        public async Task GetTable_NonExistingTableId_ReturnsNotFound()
        {
            int tableId = -999;

            await using var response = await Request.GetAsync($"/api/Table/{tableId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Table with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task GetTableInfo_ExistingTableId_ReturnsOkAndTableInfo()
        {
            int tableId = 1;

            await using var response = await Request.GetAsync($"/api/Table/info/{tableId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("tableID", out var tableID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)tableID, Is.EqualTo(tableId));
            });
        }

        [Test]
        public async Task GetTableInfo_NonExistingTableId_ReturnsNotFound()
        {
            int tableId = -999;

            await using var response = await Request.GetAsync($"/api/Table/info/{tableId}");
            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Table with given ID doesn't exist"));
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

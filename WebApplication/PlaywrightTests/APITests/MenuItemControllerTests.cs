
using Microsoft.EntityFrameworkCore;
using Models;
using Server.Models;

namespace PlaywrightTests.APITests
{
    [TestFixture]
    public class MenuItemControllerTests : PlaywrightTest
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
        public async Task PostItem_ValidData_ReturnsCreatedAtAction()
        {
            int newItemId = 13;

            await using var response = await Request.PostAsync("/api/MenuItem", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Price = 9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("menuItemID", out var menuItemID);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(201));
                Assert.That((int)menuItemID, Is.EqualTo(newItemId));
            });
        }

        [Test]
        public async Task PostItem_TooLongName_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/MenuItem", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "Menu Item that has a very looong name but at the same time it is veeeeeeeeeeery delicious 😋",
                    Price = 9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Name can't have more than 80 character"));
            });
        }

        [Test]
        public async Task PostItem_NegativePrice_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/MenuItem", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Price = -9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Price can't be negative"));
            });
        }

        [Test]
        public async Task PostItem_NonExistingCategory_ReturnsBadRequest()
        {
            await using var response = await Request.PostAsync("/api/MenuItem", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    Name = "TestName",
                    Price = 9999,
                    Category = "NotARealCateogry",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("The given category doesn't exist"));
            });
        }

        [Test]
        public async Task DeleteItem_ExistingItemId_ReturnsNoContent()
        {
            int deleteItemtId = 1;

            await using var response = await Request.DeleteAsync($"/api/MenuItem/{deleteItemtId}");

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task DeleteItem_NonExistingItemId_ReturnsNotFound()
        {
            int deleteItemtId = 999;

            await using var response = await Request.DeleteAsync($"/api/MenuItem/{deleteItemtId}");

            Assert.That(response, Has.Property("Status").EqualTo(404));
        }

        [Test]
        public async Task PutItem_ValidData_ReturnsNoContent()
        {
            int putItemId = 1;

            await using var response = await Request.PutAsync($"/api/MenuItem/{putItemId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    MenuItemID = 1,
                    Name = "TestName",
                    Price = 9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            Assert.That(response, Has.Property("Status").EqualTo(204));
        }

        [Test]
        public async Task PutItem_TooLongName_ReturnsBadRequest()
        {
            int putItemId = 1;

            await using var response = await Request.PutAsync($"/api/MenuItem/{putItemId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    MenuItemID = 1,
                    Name = "Menu Item that has a very looong name but at the same time it is veeeeeeeeeeery delicious 😋",
                    Price = 9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Name can't have more than 80 character"));
            });
        }

        [Test]
        public async Task PutItem_NegativePrice_ReturnsBadRequest()
        {
            int putItemId = 1;

            await using var response = await Request.PutAsync($"/api/MenuItem/{putItemId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    MenuItemID = 1,
                    Name = "TestName",
                    Price = -9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Price can't be negative"));
            });
        }

        [Test]
        public async Task PutItem_NonExistingCategory_ReturnsBadRequest()
        {
            int putGuestId = 1;

            await using var response = await Request.PutAsync($"/api/MenuItem/{putGuestId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    MenuItemID = 1,
                    Name = "TestName",
                    Price = 9999,
                    Category = "NotARealCategory",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("The given category doesn't exist"));
            });
        }

        [Test]
        public async Task PutItem_ItemIdsDontMatch_ReturnsBadRequest()
        {
            int putItemId = 2;

            await using var response = await Request.PutAsync($"/api/MenuItem/{putItemId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                },
                DataObject = new
                {
                    MenuItemID = 1,
                    Name = "TestName",
                    Price = 9999,
                    Category = "International",
                    HasAllergens = true,
                }
            });

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(400));
                Assert.That(jsonString, Does.Contain("Item IDs don't match"));
            });
        }

        [Test]
        public async Task GetItems_NonEmptyItems_ReturnsOk()
        {
            await using var response = await Request.GetAsync("/api/MenuItem");

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
        public async Task GetItem_ExistingItemId_ReturnsOkAndItem()
        {
            int itemId = 1;

            await using var response = await Request.GetAsync($"/api/MenuItem/{itemId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)!.AsObject();

            result.AsObject().TryGetPropertyValue("menuItemID", out var menuItemID);
            result.AsObject().TryGetPropertyValue("name", out var name);
            result.AsObject().TryGetPropertyValue("price", out var price);
            result.AsObject().TryGetPropertyValue("category", out var category);
            result.AsObject().TryGetPropertyValue("hasAllergens", out var hasAllergens);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That((int)menuItemID, Is.EqualTo(itemId));
                Assert.That((string)name, Is.EqualTo("Burger"));
                Assert.That((int)price, Is.EqualTo(350));
                Assert.That((string)category, Is.EqualTo("International"));
                Assert.That((bool)hasAllergens, Is.False);
            });
        }

        [Test]
        public async Task GetItem_NonExistingItemId_ReturnsNotFound()
        {
            int itemId = -999;

            await using var response = await Request.GetAsync($"/api/MenuItem/{itemId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(404));
                Assert.That(jsonString, Does.Contain("Item with given ID doesn't exist"));
            });
        }

        [Test]
        public async Task GetMenuCategories_CorrectCategories_ReturnsOk()
        {
            await using var response = await Request.GetAsync("/api/MenuItem/categories");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(result, Has.Count.GreaterThan(0));
                foreach (var categoryNode in result)
                {
                    var categoryName = categoryNode.ToString();

                    var isValidCategory = Enum.TryParse(typeof(Category), categoryName, out var categoryEnumValue);

                    Assert.That(isValidCategory, Is.True);
                }
            });
        }

        [Test]
        public async Task GetItemsForOrder_ForGuestWithAllergies_ReturnsOk()
        {
            int guestId = 4;

            await using var response = await Request.GetAsync($"/api/MenuItem/order/{guestId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(result, Has.Count.EqualTo(8));
            });
        }

        [Test]
        public async Task GetItemsForOrder_ForGuestWithNoAllergies_ReturnsOk()
        {
            int guestId = 3;

            await using var response = await Request.GetAsync($"/api/MenuItem/order/{guestId}");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var result = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(result, Has.Count.EqualTo(12));
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

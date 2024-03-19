
namespace PlaywrightTests.APITests
{
    [TestFixture]
    public class GuestControllerTests: PlaywrightTest
    {
        private IAPIRequestContext Request;

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
        }

        [Test]
        public async Task GetGuests_ReturnsOkAndGuests()
        {
            await using var response = await Request.GetAsync("/api/Guest");

            var body = await response.BodyAsync();
            var jsonString = Encoding.UTF8.GetString(body);
            var resultArray = JsonNode.Parse(jsonString)?.AsArray();

            Assert.Multiple(() =>
            {
                Assert.That(response, Has.Property("Status").EqualTo(200));
                Assert.That(resultArray, Has.Count.Not.EqualTo(0));
            });
        }

        [TearDown]
        public async Task TearDownAPITesting()
        {
            await Request.DisposeAsync();
        }
    }
}

using Xunit;
using Paybills.IntegrationTests.Fixtures;
using System.Net;

namespace Paybills.IntegrationTests.Controllers
{
    public class AccountControllerIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>
    {
        private readonly IntegrationTestWebApplicationFactory _integrationTestWebApplicationFactory;

        public AccountControllerIntegrationTest(IntegrationTestWebApplicationFactory integrationTestWebApplicationFactory)
        {
            _integrationTestWebApplicationFactory = integrationTestWebApplicationFactory;
        }

        [Fact]
        public async Task PostLogin_ReturnsSuccessWithValidCredentials()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/account/login", new { username = TestConstants.TestUsername, password = TestConstants.TestPassword });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
            Assert.Contains("username", content);
            Assert.Contains("token", content);
            Assert.Contains("userId", content);
        }

        [Fact]
        public async Task PostLogin_ReturnsUnauthorizedWithInvalidCredentials()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/account/login", new { username = TestConstants.TestUsername, password = "WrongPassword!" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains("Invalid username/password", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task PostRegister_ReturnsSuccessWithNewUser()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/account/register", new { username = "newuser", email = "newuser@example.com", password = "NewPassword123!" });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
            Assert.Contains("username", content);
            Assert.Contains("token", content);
            Assert.Contains("userId", content);
        }
    }
}
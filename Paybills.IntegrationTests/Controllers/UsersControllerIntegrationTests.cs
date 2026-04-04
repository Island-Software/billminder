using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Paybills.IntegrationTests.Fixtures;
using Xunit;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Paybills.API.DTOs;

namespace Paybills.IntegrationTests.Controllers
{
    public class UsersControllerIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
    {
        private readonly IntegrationTestWebApplicationFactory _integrationTestWebApplicationFactory;

        public UsersControllerIntegrationTests(IntegrationTestWebApplicationFactory integrationTestWebApplicationFactory)
        {
            _integrationTestWebApplicationFactory = integrationTestWebApplicationFactory;
        }

        [Fact]
        public async Task GetUser_ReturnsUserDetails()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            // First, login to get a token
            var loginResponse = await client.PostAsJsonAsync("/api/account/login", new { username = TestConstants.TestUsername, password = TestConstants.TestPassword });
            loginResponse.EnsureSuccessStatusCode();
            
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
            Assert.NotNull(loginContent);
            Assert.NotNull(loginContent.Token);

            // Set the authorization header with the token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent.Token);

            // Now make the authenticated request
            var response = await client.GetAsync("/api/users/1");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
            Assert.Contains("id", content);
            Assert.Contains("userName", content);
            Assert.Contains("email", content);
        }

    }
}
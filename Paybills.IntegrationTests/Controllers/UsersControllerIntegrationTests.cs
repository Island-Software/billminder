using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Paybills.IntegrationTests.Fixtures;
using Xunit;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Paybills.API.Application.DTOs.User;

namespace Paybills.IntegrationTests.Controllers
{
    public class UsersControllerIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
    {
        private readonly IntegrationTestWebApplicationFactory _integrationTestWebApplicationFactory;

        public UsersControllerIntegrationTests(IntegrationTestWebApplicationFactory integrationTestWebApplicationFactory)
        {
            _integrationTestWebApplicationFactory = integrationTestWebApplicationFactory;
        }

        private async Task<string> GetAuthToken(HttpClient client)
        {
            var loginResponse = await client.PostAsJsonAsync("/api/account/login", new { username = TestConstants.TestUsername, password = TestConstants.TestPassword });
            loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
            Assert.NotNull(loginContent);
            Assert.NotNull(loginContent.Token);

            return loginContent.Token;
        }

        private void setAuthHeader(HttpClient client, string token) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        [Fact]
        public async Task GetUser_ReturnsUserDetails()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            setAuthHeader(client, await GetAuthToken(client));

            // Now make the authenticated request
            var response = await client.GetAsync("/api/users/1");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
            Assert.Contains("\"id\":", content);
            Assert.Contains("\"userName\":", content);
            Assert.Contains("\"created\":", content);
            Assert.Contains("\"lastActive\":", content);
            Assert.Contains("\"bills\":", content);
            Assert.Contains("\"email\":", content);
        }

        [Fact]
        public async Task GetUserByName_ReturnsUserDetails()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            setAuthHeader(client, await GetAuthToken(client));

            // Now make the authenticated request
            var response = await client.GetAsync($"/api/users/name/{TestConstants.TestUsername}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
            Assert.Contains("\"id\":", content);
            Assert.Contains("\"userName\":", content);
            Assert.Contains("\"lastActive\":", content);
            Assert.Contains("\"email\":", content);
            Assert.Contains("\"password\":", content);
        }

        [Fact]
        public async Task UpdateUser_ReturnsSuccess()
        {
            var client = _integrationTestWebApplicationFactory.CreateClient();

            setAuthHeader(client, await GetAuthToken(client));

            var updateDto = new UserEditDto
            {
                Id = 1,
                UserName = TestConstants.TestUsername,
                Email = "newemail@example.com"
            };

            var response = await client.PutAsJsonAsync("/api/users/1", updateDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
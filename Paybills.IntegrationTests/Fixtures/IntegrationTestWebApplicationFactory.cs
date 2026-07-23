using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Paybills.API;
using Paybills.API.Data;
using System.Security.Cryptography;
using System.Text;
using Paybills.API.Domain.Entities;
using Paybills.API.Infrastructure.Data;

namespace Paybills.IntegrationTests.Fixtures
{
    /// <summary>
    /// Custom WebApplicationFactory for integration tests.
    /// Configures the test environment with:
    /// - Required environment variables (TOKEN_KEY, database credentials)
    /// - Temporary file-based SQLite database (instead of PostgreSQL)
    /// - Mocked external services (AWS SES)
    /// </summary>
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        /// <summary>
        /// Test database file path.
        /// Uses a temporary file that persists for the test session.
        /// Each factory instance gets its own database to avoid conflicts between test classes.
        /// </summary>
        private readonly string _dbPath;

        public IntegrationTestWebApplicationFactory()
        {
            // Create unique database path for each factory instance
            _dbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");

            // Ensure we start with a clean database
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }
        }

        /// <summary>
        /// Sets environment variables before host creation.
        /// This ensures Program.cs validation passes (TOKEN_KEY must be set).
        /// </summary>
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Set environment variables that the application expects
            Environment.SetEnvironmentVariable("TOKEN_KEY", TestConstants.TestTokenKey);
            Environment.SetEnvironmentVariable("PG_HOST", TestConstants.PostgresHost);
            Environment.SetEnvironmentVariable("PG_PORT", TestConstants.PostgresPort);
            Environment.SetEnvironmentVariable("PG_USER", TestConstants.PostgresUser);
            Environment.SetEnvironmentVariable("PG_PASSWORD", TestConstants.PostgresPassword);
            Environment.SetEnvironmentVariable("PG_DB", TestConstants.PostgresDb);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            return base.CreateHost(builder);
        }

        /// <summary>
        /// Configures the web host to use file-based SQLite database
        /// and mocks external services.
        /// </summary>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the real DbContext registration
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add file-based SQLite database for testing
                var connectionString = $"DataSource={_dbPath}";
                services.AddDbContext<DataContext>((provider, options) =>
                {
                    options.UseSqlite(connectionString);
                });

                // Build service provider and migrate the database
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                    // Create all tables on first run
                    context.Database.EnsureCreated();
                    
                    // Seed test data
                    SeedTestData(context);
                }
            });

            base.ConfigureWebHost(builder);
        }

        /// <summary>
        /// Seeds the database with test data needed for integration tests.
        /// </summary>
        private void SeedTestData(DataContext context)
        {
            // Check if test user already exists
            if (context.Users.Any(u => u.UserName == TestConstants.TestUsername.ToLower()))
            {
                return;
            }

            // Create test user with hashed password
            var testUser = new AppUser
            {
                UserName = TestConstants.TestUsername.ToLower(),
                Email = "test@example.com",
                Created = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                EmailValidated = true
            };

            // Hash password using HMACSHA512 (same method as AccountController)
            using (var hmac = new HMACSHA512())
            {
                testUser.PasswordSalt = hmac.Key;
                testUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(TestConstants.TestPassword));
            }

            context.Users.Add(testUser);
            context.SaveChanges();
        }

        public override ValueTask DisposeAsync()
        {
            // Clean up test database file
            try
            {
                if (File.Exists(_dbPath))
                {
                    File.Delete(_dbPath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }

            return base.DisposeAsync();
        }
    }
}

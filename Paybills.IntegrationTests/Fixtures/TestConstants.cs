namespace Paybills.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test environment constants and configuration values used for integration tests.
    /// </summary>
    public static class TestConstants
    {
        /// <summary>
        /// JWT signing key for test authentication tokens.
        /// Must be at least 32 bytes long for HS256 algorithm.
        /// </summary>
        public const string TestTokenKey = "this is a very large token key for hs256 token validation this is a very large token key for hs256 token validation";

        /// <summary>
        /// Database connection parameters (for in-memory SQLite, these are not used but required by Startup).
        /// </summary>
        public const string PostgresHost = "localhost";
        public const string PostgresPort = "5432";
        public const string PostgresUser = "testuser";
        public const string PostgresPassword = "testpassword";
        public const string PostgresDb = "test_billminder";

        /// <summary>
        /// Test user credentials.
        /// </summary>
        public const string TestUsername = "testuser";
        public const string TestPassword = "TestPassword123!";
    }
}

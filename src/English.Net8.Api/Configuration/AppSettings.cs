namespace English.Net8.Api.Configuration
{
    public class AuthSettings
    {
        public string Secret { get; set; }
        public int ExpiresMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string AuthCookieName { get; set; }
        public string ExpiresCookieName { get; set; }
        public string AcceptedDomainsCors { get; set; }
    }
    public class MongoSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string AccountsCollection { get; set; }
        public string RolesCollection { get; set; }
        public string UsersCollection { get; set; }
    }

    public class EmailProviderSettings
    {
        public string ApiKey { get; set; }
        public string DomainEmail { get; set; }
    }
}

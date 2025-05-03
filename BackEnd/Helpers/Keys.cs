using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Helpers
{
    public static class Keys
    {
        public const string OwnIssuer = "dotnet-user-jwts";
        public const string SectionKeys = "Authentication:Schemes:Bearer:SigningKeys";
        public const string Issuer = "Issuer";
        public const string SectionValue = "Value";

        public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration)
            => GetKey(configuration, OwnIssuer);

        public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration, string issuer)
        {
            var signingKey = configuration.GetSection(SectionKeys)
                .GetChildren()
                .SingleOrDefault(key => key[Issuer] == issuer);

            if (signingKey is not null && signingKey[SectionValue] is string keyValue)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(keyValue));
            }
        }

        public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration configuration)
        {
            var signingKeys = configuration.GetSection(SectionKeys)
                .GetChildren();

            foreach (var signingKey in signingKeys)
            {
                if (signingKey[SectionValue] is string keyValue)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(keyValue));
                }
            }
        }
    }
}

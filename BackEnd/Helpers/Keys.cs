using Microsoft.IdentityModel.Tokens;
namespace BackEnd.Helpers;

public static class Keys
{
    public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration configuration)
    {
        var section = configuration
            .GetSection("Authentication:Schemes:Bearer:SigningKeys");

        var keys = new List<SecurityKey>();

        foreach (var child in section.GetChildren())
        {
            var value = child["Value"];
            var keyId = child["Id"];

            if (string.IsNullOrWhiteSpace(value))
                continue;

            // 🔥 CLAVE: DECODIFICAR BASE64
            var keyBytes = Convert.FromBase64String(value);

            keys.Add(new SymmetricSecurityKey(keyBytes)
            {
                KeyId = keyId
            });
        }

        return keys;
    }
}

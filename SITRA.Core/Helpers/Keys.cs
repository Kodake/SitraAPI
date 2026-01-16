using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SITRA.Core.Helpers;

public static class Keys
{
    public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration configuration)
    {
        var section = configuration
            .GetSection("Authentication:Schemes:Bearer:SigningKeys");

        var keys = new List<SecurityKey>();

        // Try to load from SigningKeys section first
        foreach (var child in section.GetChildren())
        {
            var value = child["Value"];
            var keyId = child["Id"];

            if (string.IsNullOrWhiteSpace(value))
                continue;

            // ?? CLAVE: DECODIFICAR BASE64
            var keyBytes = Convert.FromBase64String(value);

            keys.Add(new SymmetricSecurityKey(keyBytes)
            {
                KeyId = keyId
            });
        }

        // Fallback to Jwt:Key if no SigningKeys found
        if (keys.Count == 0)
        {
            var jwtKey = configuration["Jwt:Key"];
            if (!string.IsNullOrWhiteSpace(jwtKey))
            {
                var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
                keys.Add(new SymmetricSecurityKey(keyBytes));
            }
        }

        return keys;
    }
}


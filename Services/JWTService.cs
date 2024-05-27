using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ThanhBuoiAPI.Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string id, string email, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, id)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
         
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = signingCredentials,
                Issuer = _configuration["Jwt:ValidIssuer"], 
                Audience = _configuration["Jwt:ValidAudience"] 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return tokenString;
        }
        public  string GetUserIdFromAuthorizationHeader(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new ArgumentException("Bearer token is missing or invalid.");
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        return GetUserIdFromToken(token);
    }

        private static string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new ArgumentException("Invalid JWT token.");
            }

            // Log the claims for debugging
            foreach (var claim in jsonToken.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            if (userIdClaim == null)
            {
                throw new ArgumentException("The token does not contain the required NameIdentifier claim.");
            }

            return userIdClaim.Value;
        }


    }
}

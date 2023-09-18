using JwtAspNet.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAspNet.Services;

public class TokenService
{
    public string Create(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(Configuration.PrivateKey);

        // Assinando o token
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        // Parte mais importante, onde vai conter as informações do token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = GenerateClaims(user),
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim("id", user.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
        claimsIdentity.AddClaim(new Claim("image", user.Image));
        user.Roles.ToList().ForEach(x => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, x)));

        return claimsIdentity;
    }
}

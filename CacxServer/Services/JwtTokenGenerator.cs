using CacxServer.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CacxServer.Services;

public static class JwtTokenGenerator
{
    public static string GenerateAccessToken(long userID, string deviceID)
    {
        TimeSpan ttl = TimeSpan.FromMinutes(30);
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userID.ToString()),
            new Claim("did", deviceID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtAccessSecret)); //LAD AUS ENV 32bit
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.Add(ttl),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken(long userID, string deviceID)
    {
        TimeSpan ttl = TimeSpan.FromDays(30);
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userID.ToString()),
            new Claim("did", deviceID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtRefreshSecret)); //LAD AUS ENV 32bit
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.Add(ttl),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

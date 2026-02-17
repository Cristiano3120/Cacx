// Ignore Spelling: Jwt

using CacxServer.Abstractions;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CacxServer.Services;

public static class JwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT access token for the specified user and device. 
    /// The token includes claims for the user ID, device ID, and a unique identifier (JTI). 
    /// The token is signed using a symmetric security key derived from an environment variable and has a time-to-live (TTL) of 30 minutes.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="deviceID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GenerateAccessToken(long userID, string deviceID)
    {
        TimeSpan ttl = TimeSpan.FromMinutes(30);
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userID.ToString()),
            new Claim("did", deviceID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        if (Env.GetString("JWT_ACCESS_TOKEN_SECRET_KEY") is not string accessToken)
        {
            throw new InvalidOperationException("Access token environment variable is not set.");
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(accessToken));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.Add(ttl),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a JWT refresh token for the specified user and device. 
    /// The token includes claims for the user ID, device ID, and a unique identifier (JTI). 
    /// The token is signed using a symmetric security key derived from an environment variable and has a time-to-live (TTL) of 30 days.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="deviceID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GenerateRefreshToken(long userID, string deviceID)
    {
        TimeSpan ttl = TimeSpan.FromDays(30);
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userID.ToString()),
            new Claim("did", deviceID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        if (Env.GetString("JWT_REFRESH_TOKEN_SECRET_KEY") is not string refreshToken)
        {
            throw new InvalidOperationException("Refresh token environment variable is not set.");
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(refreshToken));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.Add(ttl),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static JwtTokens GenerateJwtTokens(long userID, string deviceID)
        => new(refreshToken: GenerateRefreshToken(userID, deviceID), accessToken: GenerateAccessToken(userID, deviceID));   
}
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
    .AddInMemoryClients(
    [
        new Client
        {
            ClientId = "mcp-client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "mcp", "openid", "profile" },
        }
    ])
    .AddInMemoryApiResources(new[]
    {
        new ApiResource("mcp", "MCP Server API")  // <-- this will be the 'aud'
        {
            Scopes = { "mcp" },                     // link the ApiScope
            UserClaims = { ClaimTypes.Name, "email" } // include claims in access token
        }
    })
    .AddInMemoryApiScopes(new[]
    {
        new ApiScope("mcp", "MCP Server API")
        {
             UserClaims = { ClaimTypes.Name, "email" } 
        }
    })
    .AddInMemoryIdentityResources(
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ])    
    .AddTestUsers(new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password",
            Claims =
            {
                new Claim(ClaimTypes.Name, "Alice Smith"),
                new Claim("email", "alice@example.com")
            }
        }
    });
builder.WebHost.UseUrls("https://localhost:6001");

var app = builder.Build();

app.UseIdentityServer();

app.Run();

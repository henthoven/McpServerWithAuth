using IdentityModel.Client;

namespace ChatClient;

public static class LoginHelper
{
    public async static Task<string> Login(string username, string password)
    {
        // HttpClient for communication
        var client = new HttpClient();

        // Discover endpoints from metadata
        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:6001"); // IdentityServer URL
        if (disco.IsError)
        {
            Console.WriteLine($"Discovery error: {disco.Error}");
            return "";
        }

        // Request client credentials token    
        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            UserName = username,
            Password = password,
            ClientId = "mcp-client",
            ClientSecret = "secret",
            Scope = "mcp",
        });

        if (tokenResponse.IsError)
        {
            Console.WriteLine($"Token error: {tokenResponse.Error}");
            return "";
        }

        Console.WriteLine("Access Token:");
        Console.WriteLine(tokenResponse.AccessToken);
        return tokenResponse.AccessToken!;
    }
}

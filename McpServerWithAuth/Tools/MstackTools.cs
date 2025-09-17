using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Security.Claims;

namespace McpServerWithAuth.Tools;

[McpServerToolType]
public class MstackTools(IHttpContextAccessor httpContextAccessor)
{
    [McpServerTool, Description("Returns the mstack employees")]
    public string GetMstackEmployees()
    {
        var user = httpContextAccessor?.HttpContext?.User;

        var username =
            user?.FindFirst(ClaimTypes.Name)?.Value ??
            user?.FindFirst("preferred_username")?.Value ??
            user?.FindFirst("email")?.Value ??
            "anonymous";

        return $"[{username}] Hans, Mike, Gabor, Stef, Jasper, Brenda";
    }
}

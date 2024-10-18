using Microsoft.AspNetCore.Identity;

namespace AuthTest.Services.JwtService;

public interface IJwtService
{
    string CreateAuthToken(IdentityUser user);
}

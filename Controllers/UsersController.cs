using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using AuthTest.Dtos.User;
using AuthTest.Services.JwtService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers;

[Route("api/users/identity")]
[ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public class UsersController : ApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        [FromServices] UserManager<IdentityUser> userManager,
        [FromServices] IJwtService jwtService)
    {
        var user = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Email
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToArray();
            var validationResult = new ValidationProblemDetails
            {
                Title = "One or more validation errors occurred.",
                Type = "Register",
                Detail = "Error to register.",
                Status = StatusCodes.Status400BadRequest,
                Errors = errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray())
            };
            return ValidationProblem(validationResult);
        }
        var token = jwtService.CreateAuthToken(user);
        return Created("/api/users/identity/login", new AuthResponseDto { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] RegisterRequestDto request,
        [FromServices] UserManager<IdentityUser> userManager,
        [FromServices] SignInManager<IdentityUser> signInManager,
        [FromServices] IJwtService jwtService)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Credenciais inválidas");
        }
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Credenciais inválidas");
        }
        var token = jwtService.CreateAuthToken(user);
        var response = new AuthResponseDto
        {
            Token = token
        };
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPassword(
        string Email,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            return Accepted();
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        string baseUrl = "http://frontenzo.com";
        string resetPasswordUrl = $"{baseUrl}/auth/reset-password?token={token}&email={Email}";

        var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
        {
            Credentials = new NetworkCredential("3b5e6a85e75713", "ca551c4d62e4c1"),
            EnableSsl = true
        };
        smtpClient.Send("authtest@email.com", Email, "Reset Password", resetPasswordUrl);

        return Accepted();
    }

    [AllowAnonymous]
    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromQuery] string email,
        [FromQuery] string token,
        [FromBody] string newPassword,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Unauthorized();
        }
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToArray();
            var validationResult = new ValidationProblemDetails
            {
                Title = "One or more validation errors occurred.",
                Type = "Register",
                Detail = "Error to register.",
                Status = StatusCodes.Status400BadRequest,
                Errors = errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray())
            };
            return ValidationProblem(validationResult);
        }
        return Ok();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me([FromServices] UserManager<IdentityUser> userManager)
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (email is null)
        {
            return Unauthorized();
        }
        var user = await userManager.FindByEmailAsync(email);
        return Ok(user);
    }
}

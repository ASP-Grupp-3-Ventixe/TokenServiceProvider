using Microsoft.AspNetCore.Mvc;
using Presentation.Interfaces;
using Presentation.Models;
using Presentation.Services;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenService) : ControllerBase
{
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request.");

        var result = await _tokenService.GetTokenAsync(request);

        if (!result.Succeeded)
            return BadRequest(result.Message);

        return Ok(result);
    }
}

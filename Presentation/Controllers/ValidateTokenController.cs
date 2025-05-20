using Microsoft.AspNetCore.Mvc;
using Presentation.Interfaces;
using Presentation.Models;
using System.Threading.Tasks;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidateTokenController(ITokenService tokenService) : ControllerBase
{
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost]
    public async Task<IActionResult> Validate([FromBody] ValidationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid validation request");

        var result = await _tokenService.ValidateTokenAsync(request);

        if (!result.Succeeded)
            return Unauthorized("Invalid access token.");

        return Ok(result);
    }
}

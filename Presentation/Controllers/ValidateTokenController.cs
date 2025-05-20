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
    public async Task<IActionResult> Post([FromBody] ValidationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationResponse { Succeeded = false, Message = "Invalid token request." });

        var result = await _tokenService.ValidateAccessTokenAsync(request);

        if (!result.Succeeded)
            return Unauthorized("Invalid access token.");

        return Ok(result);
    }
}

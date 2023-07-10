using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingifyChallenge.Models.Requests;
using ShoppingifyChallenge.Models.Responses;
using ShoppingifyChallenge.Services;

namespace ShoppingifyChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [Route("magiclink")]
        public async Task<IActionResult> GenerateMagiclink([FromBody] MagicLinkRequest body, [FromServices] IValidator<MagicLinkRequest> magicLinkValidator)
        {
            var result = magicLinkValidator.Validate(body);
            if (!result.IsValid)
            {
                var modelStateDictionary = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelStateDictionary);
            }

            var token = await _authService.GenerateMagiclinkToken(body.Email);
            return Ok();
        }

        [HttpGet]
        [Route("magiclink/login/{token}")]
        public async Task<IActionResult> LoginWithMagiclinkToken(string token)
        {
            var result = await _authService.LoginWithMagiclinkToken(token);
            if (result.IsFailed)
            {
                return Unauthorized();
            }

            return Ok(new LoginResponse { Token = result.Value });
        }
    }
}

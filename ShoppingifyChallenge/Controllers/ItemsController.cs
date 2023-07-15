using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingifyChallenge.Models.Requests;
using ShoppingifyChallenge.Services;
using ShoppingifyChallenge.Validators;
using System.Security.Claims;

namespace ShoppingifyChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;
        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest body, [FromServices] IValidator<CreateItemRequest> validator)
        {
            var parsedResult = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
            if (!parsedResult)
            {
                return Unauthorized();
            }

            var result = validator.Validate(body);
            if (!result.IsValid)
            {
                var modelStateDictionary = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelStateDictionary);
            }

            var createResult = await _itemsService.CreateItem(userId, body.CategoryId, body.Name);
            if (createResult.IsFailed)
            {
                return BadRequest(createResult.Errors[0]);
            }

            return Ok(createResult.Value);
        }
    }
}

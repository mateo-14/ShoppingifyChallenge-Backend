using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingifyChallenge.Models.Requests;
using ShoppingifyChallenge.Services;
using System.Security.Claims;

namespace ShoppingifyChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoryService;
        public CategoriesController(ICategoriesService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var parsedResult = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
            if (!parsedResult)
            {
                return Unauthorized();
            }

            var categories = await _categoryService.GetAllCategories(userId);

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest body, [FromServices] IValidator<CreateCategoryRequest> validator)
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

            var createResult = await _categoryService.CreateCategory(userId, body.Name);
            
            if (createResult.IsFailed)
            {
                return BadRequest(createResult.Errors[0]);
            }

            return Ok(createResult.Value);
        }
    }
}

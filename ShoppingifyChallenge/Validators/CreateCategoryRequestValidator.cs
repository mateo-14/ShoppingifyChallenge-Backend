using FluentValidation;
using ShoppingifyChallenge.Models.Requests;

namespace ShoppingifyChallenge.Validators
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

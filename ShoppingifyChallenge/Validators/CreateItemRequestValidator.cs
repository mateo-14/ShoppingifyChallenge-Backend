using FluentValidation;
using ShoppingifyChallenge.Models.Requests;

namespace ShoppingifyChallenge.Validators
{
    public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
    {
        public CreateItemRequestValidator() { 
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}

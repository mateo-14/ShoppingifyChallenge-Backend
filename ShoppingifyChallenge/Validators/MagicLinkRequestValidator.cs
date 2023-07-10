using FluentValidation;
using ShoppingifyChallenge.Models.Requests;

namespace ShoppingifyChallenge.Validators
{
    public class MagicLinkRequestValidator : AbstractValidator<MagicLinkRequest>
    {
        public MagicLinkRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}

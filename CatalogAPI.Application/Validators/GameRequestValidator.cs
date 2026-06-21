using CatalogAPI.Application.Contracts.DomainErrors;
using CatalogAPI.Domain.Entities;
using CatalogAPI.Shared.Helpers;
using FluentValidation;

namespace CatalogAPI.Application.Validators
{
    public class GameRequestValidator : AbstractValidator<GameEntity>
    {
        public GameRequestValidator()
        {
            RuleFor(g => g.Name)
                .NotEmpty()
                    .WithMessage(GameErrors.Game_Error_NameCanNotBeNullOrEmpty.Description())
                .MinimumLength(8)
                    .WithMessage(GameErrors.Game_Error_NameLengthLessEight.Description());

            RuleFor(g => g.Description)
                .NotEmpty()
                    .WithMessage(GameErrors.Game_Error_DescriptionCanNotBeNullOrEmpty.Description())
                .MinimumLength(8)
                    .WithMessage(GameErrors.Game_Error_DescriptionLengthLessEight.Description());
        }
    }
}
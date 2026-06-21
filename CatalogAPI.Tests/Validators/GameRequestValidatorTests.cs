using CatalogAPI.Application.Contracts.DomainErrors;
using CatalogAPI.Application.Validators;
using CatalogAPI.Domain.Entities;
using CatalogAPI.Shared.Helpers;
using FluentValidation.TestHelper;

namespace CatalogAPI.Tests.Validators
{
    public class GameRequestValidatorTests
    {
        private readonly GameRequestValidator _validator;

        public GameRequestValidatorTests()
        {
            _validator = new GameRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var game = new GameEntity
            {
                Name = string.Empty,
                Description = "Descrição válida"
            };

            var result = _validator.TestValidate(game);

            result.ShouldHaveValidationErrorFor(g => g.Name)
                .WithErrorMessage(GameErrors.Game_Error_NameCanNotBeNullOrEmpty.Description());
        }

        [Fact]
        public void Should_Have_Error_When_Name_Has_Less_Than_Eight_Characters()
        {
            var game = new GameEntity
            {
                Name = "Game",
                Description = "Descrição válida"
            };

            var result = _validator.TestValidate(game);

            result.ShouldHaveValidationErrorFor(g => g.Name)
                .WithErrorMessage(GameErrors.Game_Error_NameLengthLessEight.Description());
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var game = new GameEntity
            {
                Name = "Game Teste",
                Description = string.Empty
            };

            var result = _validator.TestValidate(game);

            result.ShouldHaveValidationErrorFor(g => g.Description)
                .WithErrorMessage(GameErrors.Game_Error_DescriptionCanNotBeNullOrEmpty.Description());
        }

        [Fact]
        public void Should_Have_Error_When_Description_Has_Less_Than_Eight_Characters()
        {
            var game = new GameEntity
            {
                Name = "Game Teste",
                Description = "Curta"
            };

            var result = _validator.TestValidate(game);

            result.ShouldHaveValidationErrorFor(g => g.Description)
                .WithErrorMessage(GameErrors.Game_Error_DescriptionLengthLessEight.Description());
        }

        [Fact]
        public void Should_Not_Have_Error_When_Game_Is_Valid()
        {
            var game = new GameEntity
            {
                Name = "Game Teste",
                Description = "Descrição válida para o jogo",
                Price = 99.9,
                IsActive = true
            };

            var result = _validator.TestValidate(game);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
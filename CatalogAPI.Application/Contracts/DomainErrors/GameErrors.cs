using System.ComponentModel;

namespace CatalogAPI.Application.Contracts.DomainErrors
{
    public enum GameErrors
    {
        [Description("'Name' can not be null or empty!")]
        Game_Error_NameCanNotBeNullOrEmpty,

        [Description("'Name' must be at least 8 characters long!")]
        Game_Error_NameLengthLessEight,

        [Description("'Description' can not be null or empty!")]
        Game_Error_DescriptionCanNotBeNullOrEmpty,

        [Description("'Description' must be at least 8 characters long!")]
        Game_Error_DescriptionLengthLessEight,
    }
}
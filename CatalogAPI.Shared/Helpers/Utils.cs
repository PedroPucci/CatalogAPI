using System.ComponentModel;

namespace CatalogAPI.Shared.Helpers
{
    public static class Utils
    {
        public static string Description(this Enum value)
        {
            {
                DescriptionAttribute[]? array =
                    (DescriptionAttribute[]?)value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return (array != null && array.Length != 0)
                    ? array[0].Description
                    : value.ToString();
            }
        }
    }
}
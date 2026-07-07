using System.Collections.Generic;

namespace HotelStay.API.Utils
{
    public static class SeedData
    {
        public static readonly HashSet<string> DomesticCities = new(StringComparer.OrdinalIgnoreCase)
        {
            "Seattle",
            "Portland"
        };

        public static readonly HashSet<string> InternationalCities = new(StringComparer.OrdinalIgnoreCase)
        {
            "Paris",
            "Tokyo",
            "Sydney"
        };
    }
}

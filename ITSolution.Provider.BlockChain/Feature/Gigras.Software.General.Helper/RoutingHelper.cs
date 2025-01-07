using System.Text.RegularExpressions;

namespace Gigras.Software.General.Helper
{
    public static class RoutingHelper
    {
        public static string ConvertToRoutingName(string input)
        {
            // Convert to lowercase
            string lowercased = input.ToLower();

            // Replace spaces with hyphens
            string hyphenated = lowercased.Replace(" ", "-");

            // Remove any non-alphanumeric characters except hyphens
            string cleaned = Regex.Replace(hyphenated, @"[^a-z0-9\-]", "");

            return cleaned;
        }
    }
}
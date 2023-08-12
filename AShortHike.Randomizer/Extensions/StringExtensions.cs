
namespace AShortHike.Randomizer.Extensions
{
    public static class StringExtensions
    {
        public static string DisplayAsDashIfNull(this string str)
        {
            return string.IsNullOrEmpty(str) ? "---" : str;
        }
    }
}

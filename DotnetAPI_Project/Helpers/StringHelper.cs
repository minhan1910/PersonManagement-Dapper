
namespace DotnetAPI_Project.Helpers
{
    public static class StringHelper
    {
        public static string EscapeQuoteString(string s) => s.Replace("'", "''");
    }
}

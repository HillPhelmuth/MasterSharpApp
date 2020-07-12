namespace MasterSharpOpen.Client.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string GetVideoId(this string url)
        {
            return url.TrimEnd().Length >= 11 ? url.Substring(url.TrimEnd().Length - 11) : null;
        }
    }
}

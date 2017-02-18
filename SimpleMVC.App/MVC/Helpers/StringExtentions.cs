namespace SimpleMVC.App.MVC.Helpers
{
    public static class StringExtentions
    {
        public static string ToUpperFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}

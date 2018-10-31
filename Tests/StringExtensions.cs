using System;
using System.Text;

public static class StringExtensions
{
    public static StringComparison DefaultComparison = StringComparison.OrdinalIgnoreCase;
    public static string ReplaceCaseless(this string str, string oldValue, string newValue)
    {
        var sb = new StringBuilder();

        var previousIndex = 0;
        var index = str.IndexOf(oldValue, DefaultComparison);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, DefaultComparison);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }
}
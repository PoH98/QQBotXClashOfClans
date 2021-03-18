using System.Text.RegularExpressions;

namespace QQBotXClashOfClans_v2
{
    public static class ChineseDetection
    {
        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(this char c)
        {
            return cjkCharRegex.IsMatch(c.ToString());
        }

        public static bool StartsWithChinese(this string text)
        {
            return cjkCharRegex.IsMatch(text[0].ToString());
        }
    }
}

using System.Text.RegularExpressions;

namespace AskDb.Library
{
    static class CompiledExpressions
    {
        public static Regex WhitespaceAtStart = new(@"^\s+");
        public static Regex WhitespaceAtEnd = new(@"\s+$");
        public static Regex WhitespaceInMiddle = new(@"\s+");
    }
}
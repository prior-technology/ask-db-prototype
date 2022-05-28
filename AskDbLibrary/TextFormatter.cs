namespace AskDb.Library
{
    public static class FormatText
    {
        public static string AsOneLine(this string text)
        {
            var cleaned = CompiledExpressions.WhitespaceAtStart.Replace(text, "");
            cleaned = CompiledExpressions.WhitespaceAtEnd.Replace(cleaned, "");
            return CompiledExpressions.WhitespaceInMiddle.Replace(cleaned, " ");
        }
    }
}

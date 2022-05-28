using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace AskDb.Library
{
    /// <summary>
    /// HTML Converter based on HTML Agility Pack which returns a single line
    /// with heading and paragraph tags including text content but with other tags stripped out 
    /// </summary>
    public class AgilityConverter1 : IHtmlConverter
    {
        public string Convert(Stream stream)
        {
            var sb = new StringBuilder();
            var doc = new HtmlDocument();
            doc.Load(stream);
            var docNode = doc.DocumentNode;
            if (docNode == null) return "";
            var paragraphs = docNode.SelectNodes("//p|//h1|//h2|//h3|//h4|//h5|//h6");
            if (paragraphs == null) return "";
            foreach (var paragraph in paragraphs)
            {
                var cleaned = Simplify(paragraph);
                sb.Append(cleaned);
            }

            return sb.ToString();
        }
        public string Simplify(HtmlNode element)
        {
            var cleaned = element.InnerText.AsOneLine();
            if (cleaned.Length == 0) return cleaned;

            return $"<{element.Name}>{cleaned}</{element.Name}>";
        }
    }
}
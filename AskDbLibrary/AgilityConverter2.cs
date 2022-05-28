using System.IO;
using HtmlAgilityPack;

namespace AskDb.Library
{
     /// <summary>
     /// HTML Converter based on HTML Agility Pack which returns entire text from page body as a single line
     /// </summary>
    public class AgilityConverter2: IHtmlConverter
    {
        public string Convert(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);

            return Simplify(doc.DocumentNode.SelectSingleNode("//body"));
        }
        public string Simplify(HtmlNode element)
        {
            return element.InnerText.AsOneLine();
        }
    }
}
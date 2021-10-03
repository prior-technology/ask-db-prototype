using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace AskDb.Library
{
    public class FileParser
    {
        public async Task<string> GetAsText(FileInfo file)
        {
            if (IsHtmlExtension(file.Extension))
            {
                return GetTextFromHtml(file);
            }

            var streamReader = file.OpenText();
            return await streamReader.ReadToEndAsync();

        }

        /// <summary>
        /// Extracts text from an HTML formatted string
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string GetTextFromHtml(FileInfo file)
        {
            var doc = new HtmlDocument();
            using var stream = file.OpenRead();
            doc.Load(stream);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            return body.InnerText;
        }

        private static bool IsHtmlExtension(string fileExtension)
        {
            return fileExtension.Equals(".html", StringComparison.InvariantCultureIgnoreCase)
                   || fileExtension.Equals(".htm", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AskDb.Library
{
    public class FileParser
    {
        private readonly ILogger _logger;
        private readonly IHtmlConverter _htmlConverter;
        public FileParser() : this(null,null)  {  }

        public FileParser(ILogger<FileParser> logger, IHtmlConverter htmlConverter)
        {
            _logger = logger ?? NullLogger<FileParser>.Instance;
            _htmlConverter = htmlConverter ?? new AgilityConverter1();
        }
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
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetTextFromHtml(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            try
            {
                using var stream = file.OpenRead();
                return _htmlConverter.Convert(stream);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to parse file {0}", file.FullName);
                return string.Empty;
            }
        }

        private static bool IsHtmlExtension(string fileExtension)
        {
            return fileExtension.Equals(".html", StringComparison.InvariantCultureIgnoreCase)
                   || fileExtension.Equals(".htm", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

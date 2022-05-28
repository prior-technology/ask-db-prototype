using System.IO;

namespace AskDb.Library
{
    /// <summary>
    /// Interface to extract plain text for indexing from a stream
    /// </summary>
    public interface IHtmlConverter
    {
        string Convert(Stream stream);
    }
}
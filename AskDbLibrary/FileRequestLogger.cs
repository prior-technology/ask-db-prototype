using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI;

namespace AskDb.Library
{
    public class FileRequestLogger
    {
        private readonly FileStream _outputStream;

        public FileRequestLogger(FileInfo outfile)
        {
            if (outfile != null)
            {
                _outputStream = outfile.OpenWrite();
            }
            else
            {
                _outputStream = null;
            }
        }

        public async Task Log(UploadFileRequest fileRequest, IEnumerable<SearchDocument> searchDocuments)
        {
            if (_outputStream == null) return;
            try
            {
                await JsonSerializer.SerializeAsync(_outputStream,
                    new {fileRequest.FileName, fileRequest.Purpose, SearchDocuments = searchDocuments });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _outputStream.Close();
            }
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI;

namespace AskDb.Library
{
    /// <summary>
    /// This class recursively indexes a folder or single file
    /// </summary>
    public class FileIndexer
    {
        private FilesEndpoint FilesEndpoint { get; set; }
        public FileIndexer(OpenAIClient client)
        {
            FilesEndpoint = new FilesEndpoint(client);
            Pattern = "*.html";
            _fileParser = new FileParser();
        }
        public DirectoryInfo BasePath { get; set; }
        public string Pattern { get; set; }

        private readonly FileParser _fileParser;

        public async Task<FilesResponse> IndexFileAsync(FileInfo file)
        {
            var searchDocument = await GetSearchDocument(file);
            var fileRequest = new UploadFileRequest(file.Name, FilePurpose.Answers, new []{searchDocument});
            return await FilesEndpoint.UploadFileAsync(fileRequest);
        }

        private async Task<SearchDocument> GetSearchDocument(FileInfo file)
        {
            var relativePath = Path.GetRelativePath(BasePath.FullName, file.FullName);
            var metaDataJson = JsonSerializer.Serialize(new {RelativePath = relativePath});
            var fileContent = await _fileParser.GetAsText(file);
            var searchDocument = new SearchDocument {MetaData = metaDataJson, Text = fileContent};
            return searchDocument;
        }

        public async Task<FilesResponse> IndexFilesAsync(string name, IEnumerable<FileInfo> files)
        {
            var documents = new List<SearchDocument>();
            foreach (var file in files)
            {
                var searchDocument = await GetSearchDocument(file);
                documents.Add(searchDocument);
            }
         
            var fileRequest = new UploadFileRequest(name, FilePurpose.Answers, documents);
            return await FilesEndpoint.UploadFileAsync(fileRequest);
        }

        public Task<FilesResponse> Index(System.IO.FileSystemInfo path, string name = null)
        {
            switch (path)
            {
                case null:
                    throw new ArgumentNullException(nameof(path));
                case FileInfo fileInfo:
                    return IndexFileAsync(fileInfo);
            }

            name ??= path.Name;

            var directoryInfo = new DirectoryInfo(path.FullName);
            var files = directoryInfo.EnumerateFiles(Pattern);
            return IndexFilesAsync(name, files);
        }
    }
}

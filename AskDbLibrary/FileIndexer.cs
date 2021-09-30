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
        }
        public System.IO.DirectoryInfo BasePath { get; set; }
        public async Task<FilesResponse> IndexFileAsync(System.IO.FileInfo file)
        {
            var relativePath = Path.GetRelativePath(BasePath.FullName, file.FullName);
            var metaDataJson = JsonSerializer.Serialize(new {RelativePath = relativePath});
            var fileContent = await GetTextContentAsync(file);
            var searchDocument = new SearchDocument {MetaData = metaDataJson, Text = fileContent};
            var fileRequest = new UploadFileRequest(file.Name, FilePurpose.Answers, new []{searchDocument});
            return await FilesEndpoint.UploadFileAsync(fileRequest);
        }

        private Task<string> GetTextContentAsync(FileInfo file)
        {
            var streamReader = file.OpenText();
            return streamReader.ReadToEndAsync();
        }

        public void Index(System.IO.FileSystemInfo path)
        {

        }


        public void RecurseFolder(System.IO.DirectoryInfo path)
        {

        }
        
    }
}

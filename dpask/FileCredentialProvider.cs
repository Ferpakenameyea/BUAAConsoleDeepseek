using BUAADeepseekWebAPI.Credentials;

namespace dpask
{
    class FileCredentialProvider : ICredentialProvider
    {
        private readonly string _path;

        public FileCredentialProvider(string path)
        {
            _path = path;
        }

        public async Task<Credential> GetCredentialAsync()
        {
            var fileInfo = new FileInfo(_path);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Credential file not found", _path);
            }

            using var stream = new StreamReader(fileInfo.OpenRead());

            string? cookie = await stream.ReadLineAsync();
            if (cookie == null)
            {
                throw new InvalidDataException("Credential file is empty");
            }

            return new Credential { Cookie = cookie };
        }
    }
}

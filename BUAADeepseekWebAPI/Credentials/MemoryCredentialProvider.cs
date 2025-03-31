using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI.Credentials
{
    public sealed class MemoryCredentialProvider : ICredentialProvider
    {
        private readonly string _cookie;

        public MemoryCredentialProvider(string cookie)
        {
            _cookie = cookie;
        }

        public Task<Credential> GetCredentialAsync()
        {
            return Task.FromResult(new Credential { Cookie = _cookie });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI.Credentials
{
    public interface ICredentialProvider
    {
        public Task<Credential> GetCredentialAsync();
    }
}

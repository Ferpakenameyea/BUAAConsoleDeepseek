using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI.Exceptions
{
    public sealed class OutdatedException : Exception
    {
        public OutdatedException(string sourceModule) : base($"module name {sourceModule} is out-dated")
        {
            
        }
    }
}

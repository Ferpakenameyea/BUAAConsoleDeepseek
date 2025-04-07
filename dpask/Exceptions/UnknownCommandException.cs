using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask.Exceptions
{
    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string command)
            : base($"Unknown command: {command}")
        {
        }
    }
}

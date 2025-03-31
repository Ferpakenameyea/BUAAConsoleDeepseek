using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseek_Test
{
    internal class DependencyNotProvidedException : Exception
    {
        public DependencyNotProvidedException(string name) : base(
            $"Dependency with name {name} not provided")
        {
        }
    }
}

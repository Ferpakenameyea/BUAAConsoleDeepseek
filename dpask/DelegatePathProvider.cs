using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public class DelegatePathProvider : IPathProvider
    {
        private readonly Func<string> @delegate;

        public DelegatePathProvider(Func<string> @delegate)
        {
            this.@delegate = @delegate;
        }
        public string GetPath()
        {
            return this.@delegate();
        }
    }
}

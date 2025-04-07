using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public class ConstantPathProvider : IPathProvider
    {
        public string Path { get; set; }

        public ConstantPathProvider(string path)
        {
            Path = path;
        }

        public string GetPath() => Path;
    }
}

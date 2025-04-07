using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public interface IRuntimeConfig
    {
        public string UsingSessionId { get; set; }
        public void Persist();
    }
}

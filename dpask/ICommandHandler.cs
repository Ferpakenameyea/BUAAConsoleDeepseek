using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    internal interface ICommandHandler
    {
        public void Dispatch(string[] args);

        protected static void ExpectArgsCount(string[] args, int count)
        {
            if (args.Length != count)
            {
                throw new ArgumentException($"Expected {count} arguments, but got {args.Length}.");
            }
        }

        protected static void ExpectArgsCount(string[] args, int min, int max)
        {
            if (args.Length < min || args.Length > max)
            {
                throw new ArgumentException($"Expected between {min} and {max} arguments, but got {args.Length}.");
            }
        }
    }

    internal interface IAsyncCommandHandler
    {
        public Task DispatchAsync(string[] args);
        protected static void ExpectArgsCount(string[] args, int count)
        {
            if (args.Length != count)
            {
                throw new ArgumentException($"Expected {count} arguments, but got {args.Length}.");
            }
        }
        protected static void ExpectArgsCount(string[] args, int min, int max)
        {
            if (args.Length < min || args.Length > max)
            {
                throw new ArgumentException($"Expected between {min} and {max} arguments, but got {args.Length}.");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    internal class UnknownCommandException : Exception
    {
        internal string Command { get; private set; }

        public UnknownCommandException(string command) : base()
        {
            Command = command;
        }
    }

    internal class NoCommandException : Exception { }
}

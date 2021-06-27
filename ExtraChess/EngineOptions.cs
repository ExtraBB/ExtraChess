using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.UCI
{
    internal static class EngineOptions
    {
        internal static bool DebugMode { get; private set; }
        internal static string RegisterName { get; private set; }
        internal static string RegisterCode { get; private set; }

        internal static void Register(string[] args)
        {
            if(args.Length == 1 && args[0] == "later")
            {
                RegisterName = null;
                RegisterCode = null;
                return;
            }

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i] == "name" && i + 1 < args.Length)
                {
                    RegisterName = args[i + 1];
                    i++;
                }
                else if (args[i] == "code" && i + 1 < args.Length)
                {
                    RegisterCode = args[i + 1];
                    i++;
                }
                else
                {
                    throw new InvalidArgumentsException();
                }
            }
        }

        internal static void Debug(string[] args)
        {
            if (args.Length == 1)
            {
                if(args[0] == "on")
                {
                    DebugMode = true;
                    return;
                }
                else if (args[0] == "off")
                {
                    DebugMode = false;
                    return;
                }
            }

            throw new InvalidArgumentsException();
        }

        internal static void SetOption(string[] args)
        {
            throw new NotImplementedException();
        }
}

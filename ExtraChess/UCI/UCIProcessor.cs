using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtraChess.UCI
{
    public static class UCIProcessor
    {
        public static void ProcessInstruction(string instruction)
        {
            if (string.IsNullOrEmpty(instruction))
            {
                throw new NoCommandException();
            }
            string[] words = instruction.Split();

            string command = words[0];
            string[] options = words.Skip(1).ToArray();

            switch (command)
            {
                case "uci":
                    {
                        PrintUCI();
                        break;
                    }
                case "debug":
                    {
                        EngineOptions.Debug(options);
                        break;
                    }
                case "isready":
                    {
                        EngineState.IsReady();
                        break;
                    }
                case "setoption":
                    {
                        EngineOptions.SetOption(options);
                        break;
                    }
                case "register":
                    {
                        EngineOptions.Register(options);
                        break;
                    }
                case "ucinewgame":
                    {
                        EngineState.Reset();
                        break;
                    }
                case "position":
                    {
                        EngineState.SetupPosition(options);
                        break;
                    }
				case "d":
					{
						EngineState.Board?.Print();
						break;
					}
				case "go":
                    {
                        throw new NotImplementedException();
                    }
                case "stop":
                    {
						EngineState.Stop = true;
						break;
					}
                case "ponderhit":
                    {
                        throw new NotImplementedException();
                    }
                case "help":
                    {
                        Console.Write(File.ReadAllText("Resources/uci_help_text_en.txt"));
                        break;
                    }
                case "quit":
                    {
                        Environment.Exit(0);
                        break;
                    }
                default:
                    {
                        throw new UnknownCommandException(command);
                    }
            }
        }

        private static void PrintUCI()
        {
            Version engineVersion = Assembly.GetEntryAssembly().GetName().Version;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"id name ExtraChess v{engineVersion.Major}.{engineVersion.Minor}");
            stringBuilder.AppendLine($"id author Bruno Carvalhal");

            // TODO: Print supported options here

            stringBuilder.AppendLine("uciok");

            Console.Write(stringBuilder.ToString());
        }
    }
}

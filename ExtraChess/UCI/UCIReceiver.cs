using ExtraChess.Analysis;
using System;
using System.Diagnostics;
using System.Linq;

namespace ExtraChess.UCI
{
    public static class UCIReceiver
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
                        UCISender.SendEngineInfo();
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
                        ProcessGo(options);
                        break;
                    }
                case "stop":
                    {
						EngineState.Stop();
						break;
					}
                case "ponderhit":
                    {
                        throw new NotImplementedException();
                    }
                case "help":
                    {
                        UCISender.SendHelp();
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

        private static void ProcessGo(string[] args)
        {
            if(args.Length == 0)
            {
                MoveAnalyzer.StartAnalysis(EngineState.Board);
                return;
            }

            switch(args[0])
            {
                case "perft": 
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        UCISender.SendPerft(PerftAnalyzer.PerftDivide(EngineState.Board, int.Parse(args[1])), watch.ElapsedMilliseconds);
                        watch.Stop();
                        return;
                    }
                case "movetime":
                    {
                        MoveAnalyzer.StartAnalysis(EngineState.Board, long.Parse(args[1]));
                        return;
                    }
                case "infinite":
                    {
                        MoveAnalyzer.StartAnalysis(EngineState.Board);
                        return;
                    }
            }
        }
    }
}

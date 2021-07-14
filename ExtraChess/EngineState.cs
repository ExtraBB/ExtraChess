using ExtraChess.Analysis;
using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.Moves;
using ExtraChess.UCI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    internal static class EngineState
    {
        internal static Board Board { get; private set; }
        internal static bool Ready { get => !Search.IsSearching && !MoveGenerator.IsGenerating && !BoardGenerator.IsGenerating; }

        internal static void Initialize()
        {
            Reset();
            Search.BestMoveFound += Search_BestMoveFound;
        }

        private static void Search_BestMoveFound(Move move)
        {
            UCISender.SendBestMove(move);
        }

        internal static void Stop()
        {
            Search.Stop();
        }

        internal static void IsReady()
        {
            Task.Run(async () =>
            {
                while (!Ready)
                {
                    await Task.Delay(10);
                }
                Console.WriteLine("readyok");
            }).Wait();
        }

        internal static void SetupPosition(params string[] uciArgs)
        {
            Board = BoardGenerator.GenerateBoardFromUCIPosition(uciArgs);
        }

        internal static void Reset()
        {
            Board = null;
        }
    }
}

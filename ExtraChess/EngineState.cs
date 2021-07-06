using ExtraChess.Analysis;
using ExtraChess.Models;
using ExtraChess.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    internal static class EngineState
    {
        internal static bool Ready { get; set; } = true;

        internal static void Stop()
        {
            MoveAnalyzer.StopCalculating();
        }

        internal static Board Board { get; private set; }

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
            try
            {
                Ready = false;

                int movesArgumentIndex = Array.IndexOf(uciArgs, "moves");

                if (uciArgs[0] == "startpos")
                {
                    Board = new Board();
                }
                else if (uciArgs[0] == "fen")
                {
                    string fen = movesArgumentIndex != -1
                        ? string.Join(' ', uciArgs.Skip(1).Take(movesArgumentIndex - 1))
                        : string.Join(' ', uciArgs.Skip(1));
                    Board = new Board(fen);
                }

                if(movesArgumentIndex != -1)
                {
                    foreach (string uciMove in uciArgs.Skip(movesArgumentIndex + 1))
                    {
                        IEnumerable<Move> generatedMoves = MoveGenerator.GenerateMoves(Board);
                        Move move = Move.UCIMoveToMove(generatedMoves, uciMove);
                        if (move == null)
                        {
                            break;
                        }
                        Board.MakeMove(move);
                    }
                }
            }
            finally
            {
                Ready = true;
            }
        }

        internal static void Reset()
        {
            Board = null;
            Ready = true;
        }
    }
}

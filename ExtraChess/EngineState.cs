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

        internal static void SetupPosition(string[] uciArgs)
        {
            try
            {
                Ready = false;

                int movesArgumentIndex = Array.IndexOf(uciArgs, "moves");
                if(movesArgumentIndex != -1)
                {
                    string fen = string.Join(' ', uciArgs.Take(movesArgumentIndex));
                    Board = fen == "startpos" ? new Board() : new Board(fen);

                    foreach(string uciMove in uciArgs.Skip(movesArgumentIndex + 1))
                    {
                        IEnumerable<Move> generatedMoves = MoveGenerator.GenerateMoves(Board);
                        Move move = Move.UCIMoveToMove(generatedMoves, uciMove);
                        if(move == null)
                        {
                            break;
                        }
                        Board.MakeMove(move);
                    }
                }
                else
                {
                    string fen = string.Join(' ', uciArgs);
                    Board = fen == "startpos" ? new Board() : new Board(fen);
                }
            }
            finally
            {
                Ready = true;
            }
        }
    }
}

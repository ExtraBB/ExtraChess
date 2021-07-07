using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.Moves;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class MoveAnalyzer
    {
        private static Random random = new Random();

        public static bool IsAnalyzing { get; private set; }
        public delegate void BestMoveFoundEventHandler(Move move);
        public static event BestMoveFoundEventHandler BestMoveFound;

        public static void StartAnalysis(Board board, long calculateForMillis = long.MaxValue)
        {
            if(IsAnalyzing)
            {
                return;
            }

            try
            {
                IsAnalyzing = true;

                Stopwatch watch = new Stopwatch();
                watch.Start();

                while (IsAnalyzing && watch.ElapsedMilliseconds < calculateForMillis)
                {
                    Move bestMove = GetRandomMove(board);
                    BestMoveFound?.Invoke(bestMove);
                    break;
                }

                watch.Stop();
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        public static void StopAnalysis()
        {
            IsAnalyzing = false;
        }

        private static Move GetRandomMove(Board board)
        {
            Move[] moves = MoveGenerator.GenerateMoves(board).ToArray();
            return moves[random.Next(moves.Length)];
        }

        public static bool IsLegalMove(Board board, Move move, Player player)
        {
            Board copy = board.PreviewMove(move);
            return player == Player.White
                ? !copy.SquareIsInCheck(copy.WKing, player)
                : !copy.SquareIsInCheck(copy.BKing, player);
        }
    }
}

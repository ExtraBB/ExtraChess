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
        private static bool isCalculating = false;

        public static void StartCalculating(Board board, long calculateForMillis = long.MaxValue)
        {
            if(isCalculating)
            {
                return;
            }

            isCalculating = true;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while(isCalculating && watch.ElapsedMilliseconds < calculateForMillis)
            {
                PrintRandomMove(board);
                break;
            }

            watch.Stop();
            isCalculating = false;
        }

        public static void StopCalculating()
        {
            isCalculating = false;
        }

        private static void PrintRandomMove(Board board)
        {
            Move[] moves = MoveGenerator.GenerateMoves(board).ToArray();
            Console.WriteLine($"bestmove {moves[random.Next(moves.Length)].ToUCIMove()}");
        }
    }
}

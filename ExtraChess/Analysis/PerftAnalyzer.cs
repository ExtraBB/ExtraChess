using ExtraChess.Models;
using ExtraChess.Moves;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class PerftAnalyzer
    {
        public static ulong Perft(Board board, int depth)
        {
            var moves = MoveGenerator.GenerateMoves(board).ToArray();

            if (depth == 1)
            {
                return (ulong)moves.Length;
            }
            else if(depth == 0)
            {
                return 1;
            }

            ulong total = 0;
            for (int i = 0; i < moves.Length; i++)
            {
                total += Perft(board.PreviewMove(moves[i]), depth - 1);
            }

            return total;
        }

        public static ulong PerftConcurrent(Board board, int depth)
        {
            var moves = MoveGenerator.GenerateMoves(board).ToArray();

            if (depth == 1)
            {
                return (ulong)moves.Length;
            }

            ulong[] totals = new ulong[moves.Length];

            Parallel.For(0, moves.Length, i =>
            {
                totals[i] = Perft(board.PreviewMove(moves[i]), depth - 1);
            });

            return totals.Length > 0 ? totals.Aggregate((a, b) => a + b) : 0;
        }

        public static List<(Move, ulong)> PerftDivide(Board board, int depth)
        {
            Move[] moves = MoveGenerator.GenerateMoves(board).ToArray();
            (Move, ulong)[] result = new (Move, ulong)[moves.Length];

            Parallel.For(0, moves.Length, i =>
            {
                result[i] = (moves[i], Perft(board.PreviewMove(moves[i]), depth - 1));
            });

            return result.ToList();
        }
    }
}

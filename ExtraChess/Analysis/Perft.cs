using ExtraChess.Generators;
using ExtraChess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class Perft
    {
        public static ulong PerftSync(Board board, int depth)
        {
            List<Move> moves = MoveGenerator.GenerateMoves(board);

            if (depth == 1)
            {
                return (ulong)moves.Count;
            }
            else if(depth == 0)
            {
                return 1;
            }

            ulong total = 0;
            for (int i = 0; i < moves.Count; i++)
            {
                board.MakeMove(moves[i]);
                total += PerftSync(board, depth - 1);
                board.UnmakeMove();
            }

            return total;
        }

        public static ulong PerftConcurrent(Board board, int depth)
        {
            Move[] moves = MoveGenerator.GenerateMoves(board).ToArray();

            if (depth == 1)
            {
                return (ulong)moves.Length;
            }

            ulong[] totals = new ulong[moves.Length];

            Parallel.For(0, moves.Length, i =>
            {
                Board clone = board.Clone();
                clone.MakeMove(moves[i]);
                totals[i] = PerftSync(clone, depth - 1);
                clone.UnmakeMove();
            });

            return totals.Length > 0 ? totals.Aggregate((a, b) => a + b) : 0;
        }

        public static List<(Move, ulong)> PerftDivide(Board board, int depth)
        {
            Move[] moves = MoveGenerator.GenerateMoves(board).ToArray();
            (Move, ulong)[] result = new (Move, ulong)[moves.Length];

            Parallel.For(0, moves.Length, i =>
            {
                Board clone = board.Clone();
                clone.MakeMove(moves[i]);
                result[i] = (moves[i], PerftSync(clone, depth - 1));
                clone.UnmakeMove();
            });

            return result.ToList();
        }
    }
}

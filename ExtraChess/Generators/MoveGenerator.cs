using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtraChess.Analysis;
using ExtraChess.Models;
using ExtraChess.Moves;

namespace ExtraChess.Generators
{
    public static class MoveGenerator
    {
        public static bool IsGenerating { get; private set; }

        public static IEnumerable<Move> GenerateMovesFromBitboard(UInt64 bitboard, int position, Piece piece)
        {
            List<Move> moves = new List<Move>(32);
            for (int i = 0; i < 64; i++)
            {
                if (bitboard.NthBitSet(i))
                {
                    moves.Add(new Move(piece, position, i));
                }
            }
            return moves;
        }

        public static IEnumerable<Move> GenerateMoves(Board board)
        {
            try
            {
                IsGenerating = true;
                Magics.Initialize();
                if (board.State.CurrentPlayer == Player.Black)
                {
                    return PawnMoves.CalculateBPawnMoves(board)
                        .Concat(SlidingMoves.CalculateBBishopMoves(board))
                        .Concat(SlidingMoves.CalculateBRookMoves(board))
                        .Concat(SlidingMoves.CalculateBQueenMoves(board))
                        .Concat(KingMoves.CalculateBKingMoves(board))
                        .Concat(KnightMoves.CalculateBKnightMoves(board))
                        .Where(move => board.IsLegalMove(move));
                }
                else if (board.State.CurrentPlayer == Player.White)
                {
                    return PawnMoves.CalculateWPawnMoves(board)
                        .Concat(SlidingMoves.CalculateWBishopMoves(board))
                        .Concat(SlidingMoves.CalculateWRookMoves(board))
                        .Concat(SlidingMoves.CalculateWQueenMoves(board))
                        .Concat(KingMoves.CalculateWKingMoves(board))
                        .Concat(KnightMoves.CalculateWKnightMoves(board))
                        .Where(move => board.IsLegalMove(move));
                }
                return null;
            }
            finally
            {
                IsGenerating = false;
            }
        }
    }
}
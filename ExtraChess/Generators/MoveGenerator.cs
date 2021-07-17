using System;
using System.Collections.Generic;
using ExtraChess.Models;
using ExtraChess.Moves;

namespace ExtraChess.Generators
{
    public static class MoveGenerator
    {
        public static bool IsGenerating { get; private set; }

        public static List<Move> GenerateMovesFromBitboard(UInt64 bitboard, int position, Piece piece)
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

        public static List<Move> GenerateMoves(Board board)
        {
            try
            {
                IsGenerating = true;
                Magics.Initialize();
                List<Move> result = new List<Move>(64);
                if (board.State.CurrentPlayer == Player.Black)
                {
                    AddIfLegal(board, result, PawnMoves.CalculateBPawnMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateBBishopMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateBRookMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateBQueenMoves(board));
                    AddIfLegal(board, result, KingMoves.CalculateBKingMoves(board));
                    AddIfLegal(board, result, KnightMoves.CalculateBKnightMoves(board));
                }
                else if (board.State.CurrentPlayer == Player.White)
                {
                    AddIfLegal(board, result, PawnMoves.CalculateWPawnMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateWBishopMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateWRookMoves(board));
                    AddIfLegal(board, result, SlidingMoves.CalculateWQueenMoves(board));
                    AddIfLegal(board, result, KingMoves.CalculateWKingMoves(board));
                    AddIfLegal(board, result, KnightMoves.CalculateWKnightMoves(board));
                }
                return result;
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private static void AddIfLegal(Board board, List<Move> result, IEnumerable<Move> movesToCheck)
        {
            foreach (Move move in movesToCheck)
            {
                if (board.IsLegalMove(move))
                {
                    result.Add(move);
                }
            }
        }
    }
}
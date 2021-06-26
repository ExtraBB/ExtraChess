using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtraChess.Models;
using ExtraChess.Moves;

namespace ExtraChess.Services
{
    public static class MoveService
    {

        public static IEnumerable<Move> CreateMovesFromBitboard(UInt64 bitboard, int position, Piece piece)
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

        public static IEnumerable<Move> GetAllPossibleMoves(Board board, Move lastMove)
        {
            if (board.CurrentPlayer == Player.Black)
            {
                return PawnMoves.CalculateBPawnMoves(board, lastMove)
                    .Concat(SlidingMoves.CalculateBBishopMoves(board))
                    .Concat(SlidingMoves.CalculateBRookMoves(board))
                    .Concat(SlidingMoves.CalculateBQueenMoves(board))
                    .Concat(KingMoves.CalculateBKingMoves(board))
                    .Concat(KnightMoves.CalculateBKnightMoves(board))
                    .Where(move => IsLegalMove(board, move, Player.Black));
            }
            else if (board.CurrentPlayer == Player.White)
            {
                return PawnMoves.CalculateWPawnMoves(board, lastMove)
                    .Concat(SlidingMoves.CalculateWBishopMoves(board))
                    .Concat(SlidingMoves.CalculateWRookMoves(board))
                    .Concat(SlidingMoves.CalculateWQueenMoves(board))
                    .Concat(KingMoves.CalculateWKingMoves(board))
                    .Concat(KnightMoves.CalculateWKnightMoves(board))
                    .Where(move => IsLegalMove(board, move, Player.White));
            }
            return null;
        }

        public static bool IsLegalMove(Board board, Move move, Player player)
        {
            Board copy = board.PreviewMove(move);
            return player == Player.White
                ? !copy.SquareIsInCheck(copy.WKing, player)
                : !copy.SquareIsInCheck(copy.BKing, player);

        }

        public static ulong Perft(Board board, int depth, Move lastMove = null)
        {
            var moves = GetAllPossibleMoves(board, lastMove).ToArray();

            if (depth == 1)
            {
                return (ulong)moves.Length;
            }

            ulong total = 0;
            for (int i = 0; i < moves.Length; i++)
            {
                total += Perft(board.PreviewMove(moves[i]), depth - 1, moves[i]);
            }

            return total;
        }

        public static ulong PerftConcurrent(Board board, int depth, Move lastMove = null)
        {
            var moves = GetAllPossibleMoves(board, lastMove).ToArray();

            if (depth == 1)
            {
                return (ulong)moves.Length;
            }

            ulong[] totals = new ulong[moves.Length];

            Parallel.For(0, moves.Length, i =>
             {
                 totals[i] = Perft(board.PreviewMove(moves[i]), depth - 1, moves[i]);
             });

            return totals.Length > 0 ? totals.Aggregate((a, b) => a + b) : 0;
        }
    }
}
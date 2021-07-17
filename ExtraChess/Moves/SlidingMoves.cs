using ExtraChess.Generators;
using ExtraChess.Models;
using System;
using System.Collections.Generic;

namespace ExtraChess.Moves
{
    public static class SlidingMoves 
    {
        public static List<Move> CalculateBishopMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            Color color = board.State.CurrentPlayer.ToColor();
            Piece piece = PieceType.Bishop.ToPiece(color);
            foreach (int i in board.PositionsByPiece[piece])
            {
                UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)color];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, piece));
            }

            return moves;
        }

        public static List<Move> CalculateRookMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            Color color = board.State.CurrentPlayer.ToColor();
            Piece piece = PieceType.Rook.ToPiece(color);
            foreach (int i in board.PositionsByPiece[piece])
            {
                UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)color];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, piece));
            }

            return moves;
        }

        public static List<Move> CalculateQueenMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            Color color = board.State.CurrentPlayer.ToColor();
            Piece piece = PieceType.Queen.ToPiece(color);
            foreach (int i in board.PositionsByPiece[piece])
            {
                UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)color];
                UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)color];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, piece));
            }

            return moves;
        }

        public static UInt64 GetBishopAttackMap(List<int> bishops, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in bishops)
            {
                allAttacks |= Magics.GetBishopAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitBishopAttackMap(List<int> bishops, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in bishops)
            {
                allAttacks.Add((i, Magics.GetBishopAttacks(occupied, i)));
            }

            return allAttacks;
        }

        public static UInt64 GetRookAttackMap(List<int> rooks, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in rooks)
            {
                allAttacks |= Magics.GetRookAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitRookAttackMap(List<int> rooks, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in rooks)
            {
                allAttacks.Add((i, Magics.GetRookAttacks(occupied, i)));
            }

            return allAttacks;
        }

        public static UInt64 GetQueenAttackMap(List<int> queens, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in queens)
            {
                allAttacks |= Magics.GetRookAttacks(occupied, i);
                allAttacks |= Magics.GetBishopAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitQueenAttackMap(List<int> queens, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in queens)
            {
                allAttacks.Add((i, Magics.GetRookAttacks(occupied, i) | Magics.GetBishopAttacks(occupied, i)));
            }

            return allAttacks;
        }
    }
}
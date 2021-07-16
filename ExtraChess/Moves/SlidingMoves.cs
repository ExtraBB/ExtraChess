using ExtraChess.Generators;
using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraChess.Moves
{
    public static class SlidingMoves 
    {
        public static IEnumerable<Move> CalculateWBishopMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.WBishop].GetBitsSet())
            {
                UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WBishop));
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBBishopMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.BBishop].GetBitsSet())
            {
                UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BBishop));
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWRookMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.WRook].GetBitsSet())
            {
                UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WRook));
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBRookMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.BRook].GetBitsSet())
            {
                UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BRook));
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWQueenMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.WQueen].GetBitsSet())
            {
                UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.WQueen));
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBQueenMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            foreach (int i in board.BoardByPiece[(int)Piece.BQueen].GetBitsSet())
            {
                UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.BQueen));
            }

            return moves;
        }

        public static UInt64 GetBishopAttackMap(UInt64 bishops, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in bishops.GetBitsSet())
            {
                allAttacks |= Magics.GetBishopAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitBishopAttackMap(UInt64 bishops, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in bishops.GetBitsSet())
            {
                allAttacks.Add((i, Magics.GetBishopAttacks(occupied, i)));
            }

            return allAttacks;
        }

        public static UInt64 GetRookAttackMap(UInt64 rooks, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in rooks.GetBitsSet())
            {
                allAttacks |= Magics.GetRookAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitRookAttackMap(UInt64 rooks, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in rooks.GetBitsSet())
            {
                allAttacks.Add((i, Magics.GetRookAttacks(occupied, i)));
            }

            return allAttacks;
        }

        public static UInt64 GetQueenAttackMap(UInt64 queens, UInt64 occupied)
        {
            UInt64 allAttacks = 0;

            foreach (int i in queens.GetBitsSet())
            {
                allAttacks |= Magics.GetRookAttacks(occupied, i);
                allAttacks |= Magics.GetBishopAttacks(occupied, i);
            }

            return allAttacks;
        }

        public static List<(int position, UInt64 attack)> GetSplitQueenAttackMap(UInt64 queens, UInt64 occupied)
        {
            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();

            foreach (int i in queens.GetBitsSet())
            {
                allAttacks.Add((i, Magics.GetRookAttacks(occupied, i) | Magics.GetBishopAttacks(occupied, i)));
            }

            return allAttacks;
        }
    }
}
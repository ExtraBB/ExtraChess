using ExtraChess.Generators;
using ExtraChess.Models;
using System;
using System.Collections.Generic;

namespace ExtraChess.Moves
{
    public static class SlidingMoves 
    {
        public static List<Move> CalculateWBishopMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.WBishop])
            {
                UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WBishop));
            }

            return moves;
        }

        public static List<Move> CalculateBBishopMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.BBishop])
            {
                UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BBishop));
            }

            return moves;
        }

        public static List<Move> CalculateWRookMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.WRook])
            {
                UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WRook));
            }

            return moves;
        }

        public static List<Move> CalculateBRookMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.BRook])
            {
                UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BRook));
            }

            return moves;
        }

        public static List<Move> CalculateWQueenMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.WQueen])
            {
                UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.White];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.WQueen));
            }

            return moves;
        }

        public static List<Move> CalculateBQueenMoves(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            foreach (int i in board.PositionsByPiece[Piece.BQueen])
            {
                UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BoardByColor[(int)Color.Black];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.BQueen));
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
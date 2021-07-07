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

            for(int i = 0; i < 64; i++)
            {
                if(board.WBishops.NthBitSet(i))
                {
                    UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.WhitePieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WBishop));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBBishopMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            for(int i = 0; i < 64; i++)
            {
                if(board.BBishops.NthBitSet(i))
                {
                    UInt64 attacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BlackPieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BBishop));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWRookMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            for(int i = 0; i < 64; i++)
            {
                if(board.WRooks.NthBitSet(i))
                {
                    UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.WhitePieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WRook));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBRookMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            for(int i = 0; i < 64; i++)
            {
                if(board.BRooks.NthBitSet(i))
                {
                    UInt64 attacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BlackPieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BRook));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWQueenMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            for(int i = 0; i < 64; i++)
            {
                if(board.WQueen.NthBitSet(i))
                {
                    UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.WhitePieces;
                    UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.WhitePieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.WQueen));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBQueenMoves(Board board) 
        {
            IEnumerable<Move> moves = new List<Move>(32);

            for(int i = 0; i < 64; i++)
            {
                if(board.BQueen.NthBitSet(i))
                {
                    UInt64 bishopAttacks = Magics.GetBishopAttacks(board.Occupied, i) & ~board.BlackPieces;
                    UInt64 rookAttacks = Magics.GetRookAttacks(board.Occupied, i) & ~board.BlackPieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(bishopAttacks | rookAttacks, i, Piece.BQueen));
                }
            }

            return moves;
        }

        public static UInt64 GetBishopAttackMap(UInt64 bishops, UInt64 occupied, UInt64 ownPieces)
        {
            UInt64 allAttacks = 0;
            for(int i = 0; i < 64; i++)
            {
                if(bishops.NthBitSet(i))
                {
                    allAttacks |= Magics.GetBishopAttacks(occupied, i) & ~ownPieces;
                }
            }
            return allAttacks;
        }

        public static UInt64 GetRookAttackMap(UInt64 rooks, UInt64 occupied, UInt64 ownPieces)
        {
            UInt64 allAttacks = 0;
            for(int i = 0; i < 64; i++)
            {
                if(rooks.NthBitSet(i))
                {
                    allAttacks |= Magics.GetRookAttacks(occupied, i) & ~ownPieces;
                }
            }
            return allAttacks;
        }

        public static UInt64 GetQueenAttackMap(UInt64 queen, UInt64 occupied, UInt64 ownPieces)
        {
            UInt64 allAttacks = 0;
            for(int i = 0; i < 64; i++)
            {
                if(queen.NthBitSet(i))
                {
                    allAttacks |= Magics.GetRookAttacks(occupied, i) & ~ownPieces;
                    allAttacks |= Magics.GetBishopAttacks(occupied, i) & ~ownPieces;
                }
            }
            return allAttacks;
        }
    }
}
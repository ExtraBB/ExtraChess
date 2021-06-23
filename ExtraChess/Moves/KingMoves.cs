using ExtraChess.Models;
using ExtraChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraChess.Moves
{
    public static class KingMoves 
    {
        private static UInt64[] KingMovesLookupTable;

        public static IEnumerable<Move> CalculateWKingMoves(Board board)
        {
            if(KingMovesLookupTable == null)
            {
                GenerateKingMoves();
            }
            
            int position = board.WKing.GetLS1BIndex();
            return MoveService.CreateMovesFromBitboard(KingMovesLookupTable[position] & ~board.WhitePieces, position, Piece.WKing).Concat(GetCastlingMoves(board, Piece.WKing));
        }

        public static IEnumerable<Move> CalculateBKingMoves(Board board)
        {
            if(KingMovesLookupTable == null)
            {
                GenerateKingMoves();
            }
            
            int position = board.BKing.GetLS1BIndex();
            return MoveService.CreateMovesFromBitboard(KingMovesLookupTable[position] & ~board.BlackPieces, position, Piece.BKing).Concat(GetCastlingMoves(board, Piece.BKing));
        }

        public static UInt64 GetKingAttackMap(UInt64 king, UInt64 ownPieces)
        {
            if(KingMovesLookupTable == null)
            {
                GenerateKingMoves();
            }

            int position = king.GetLS1BIndex();
            return KingMovesLookupTable[position] & ~ownPieces;
        }

        private static void GenerateKingMoves()
        {
            KingMovesLookupTable = new UInt64[64];
            for(int i = 0; i < 64; i++)
            {
                UInt64 kingPosition = 1UL << i;
                UInt64 kingClippedH = kingPosition & ~Board.HFile; 
                UInt64 kingClippedA = kingPosition & ~Board.AFile; 

                UInt64 nw = kingClippedA << 7; 
                UInt64 n = kingPosition << 8; 
                UInt64 ne = kingClippedH << 9; 
                UInt64 e = kingClippedH << 1; 

                UInt64 se = kingClippedH >> 7; 
                UInt64 s = kingPosition >> 8; 
                UInt64 sw = kingClippedA >> 9; 
                UInt64 w = kingClippedA >> 1; 

                KingMovesLookupTable[i] = nw | n | ne | e | se | s | sw | w; 
            }
        }

        private static IEnumerable<Move> GetCastlingMoves(Board board, Piece piece)
        {
            List<Move> moves = new List<Move>(4);

            if(piece == Piece.WKing && !board.WKingMoved && board.WKing.NthBitSet((int)Square.E1))
            {
                bool emptyLeft = (0UL.SetBit((int)Square.B1).SetBit((int)Square.C1).SetBit((int)Square.D1) & board.Occupied) == 0;
                if(emptyLeft && !board.WRookLeftMoved && board.WRooks.NthBitSet((int)Square.A1) && !board.SquareIsInCheck(Square.C1, Player.White) && !board.SquareIsInCheck(Square.D1, Player.White) && !board.SquareIsInCheck(Square.E1, Player.White))
                {
                    moves.Add(new Move(Piece.WKing, Square.E1, Square.C1, specialMove: SpecialMove.Castling));
                }

                bool emptyRight = (0UL.SetBit((int)Square.F1).SetBit((int)Square.G1) & board.Occupied) == 0;
                if(emptyRight && !board.WRookRightMoved && board.WRooks.NthBitSet((int)Square.H1) && !board.SquareIsInCheck(Square.E1, Player.White) && !board.SquareIsInCheck(Square.F1, Player.White) && !board.SquareIsInCheck(Square.G1, Player.White))
                {
                    moves.Add(new Move(Piece.WKing, Square.E1, Square.G1, specialMove: SpecialMove.Castling));
                }
            }
            else if(piece == Piece.BKing && !board.BKingMoved && board.BKing.NthBitSet((int)Square.E8))
            {
                bool emptyLeft = (0UL.SetBit((int)Square.B8).SetBit((int)Square.C8).SetBit((int)Square.D8) & board.Occupied) == 0;
                if(emptyLeft && !board.BRookLeftMoved && board.BRooks.NthBitSet((int)Square.A8) && !board.SquareIsInCheck(Square.C8, Player.Black) && !board.SquareIsInCheck(Square.D8, Player.Black) && !board.SquareIsInCheck(Square.E8, Player.Black))
                {
                    moves.Add(new Move(Piece.BKing, Square.E8, Square.C8, specialMove: SpecialMove.Castling));
                }

                bool emptyRight = (0UL.SetBit((int)Square.F8).SetBit((int)Square.G8) & board.Occupied) == 0;
                if(emptyRight && !board.BRookRightMoved && board.BRooks.NthBitSet((int)Square.H8) && !board.SquareIsInCheck(Square.E8, Player.Black) && !board.SquareIsInCheck(Square.F8, Player.Black) && !board.SquareIsInCheck(Square.G8, Player.Black))
                {
                    moves.Add(new Move(Piece.BKing, Square.E8, Square.G8, specialMove: SpecialMove.Castling));
                }
            } 

            return moves;
        }
    }
}
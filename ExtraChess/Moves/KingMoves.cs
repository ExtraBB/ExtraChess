using ExtraChess.Generators;
using ExtraChess.Models;
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
            
            int position = board.BoardByPiece[(int)Piece.WKing].GetLS1BIndex();
            return MoveGenerator.GenerateMovesFromBitboard(KingMovesLookupTable[position] & ~board.BoardByColor[(int)Color.White], position, Piece.WKing).Concat(GetCastlingMoves(board, Piece.WKing));
        }

        public static IEnumerable<Move> CalculateBKingMoves(Board board)
        {
            if(KingMovesLookupTable == null)
            {
                GenerateKingMoves();
            }
            
            int position = board.BoardByPiece[(int)Piece.BKing].GetLS1BIndex();
            return MoveGenerator.GenerateMovesFromBitboard(KingMovesLookupTable[position] & ~board.BoardByColor[(int)Color.Black], position, Piece.BKing).Concat(GetCastlingMoves(board, Piece.BKing));
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
                UInt64 kingClippedH = kingPosition & ~Constants.HFile; 
                UInt64 kingClippedA = kingPosition & ~Constants.AFile; 

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

            if(piece == Piece.WKing)
            {
                if (board.State.WCanCastleQueenSide)
                {
                    bool emptyLeft = (0UL.SetBit((int)Square.B1).SetBit((int)Square.C1).SetBit((int)Square.D1) & board.Occupied) == 0;
                    if (emptyLeft && !board.SquareIsInCheck(Square.C1, Player.White) && !board.SquareIsInCheck(Square.D1, Player.White) && !board.SquareIsInCheck(Square.E1, Player.White))
                    {
                        moves.Add(new Move(Piece.WKing, Square.E1, Square.C1, specialMove: SpecialMove.Castling));
                    }
                }

                if (board.State.WCanCastleKingSide)
                {
                    bool emptyRight = (0UL.SetBit((int)Square.F1).SetBit((int)Square.G1) & board.Occupied) == 0;
                    if (emptyRight && !board.SquareIsInCheck(Square.E1, Player.White) && !board.SquareIsInCheck(Square.F1, Player.White) && !board.SquareIsInCheck(Square.G1, Player.White))
                    {
                        moves.Add(new Move(Piece.WKing, Square.E1, Square.G1, specialMove: SpecialMove.Castling));
                    }
                }
            }
            else if(piece == Piece.BKing)
            {
                if (board.State.BCanCastleQueenSide)
                {
                    bool emptyLeft = (0UL.SetBit((int)Square.B8).SetBit((int)Square.C8).SetBit((int)Square.D8) & board.Occupied) == 0;
                    if (emptyLeft && !board.SquareIsInCheck(Square.C8, Player.Black) && !board.SquareIsInCheck(Square.D8, Player.Black) && !board.SquareIsInCheck(Square.E8, Player.Black))
                    {
                        moves.Add(new Move(Piece.BKing, Square.E8, Square.C8, specialMove: SpecialMove.Castling));
                    }
                }

                if (board.State.BCanCastleKingSide)
                {
                    bool emptyRight = (0UL.SetBit((int)Square.F8).SetBit((int)Square.G8) & board.Occupied) == 0;
                    if (emptyRight && !board.SquareIsInCheck(Square.E8, Player.Black) && !board.SquareIsInCheck(Square.F8, Player.Black) && !board.SquareIsInCheck(Square.G8, Player.Black))
                    {
                        moves.Add(new Move(Piece.BKing, Square.E8, Square.G8, specialMove: SpecialMove.Castling));
                    }
                }
            } 

            return moves;
        }
    }
}
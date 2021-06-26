using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraChess.Moves
{
    public static class PawnMoves 
    {
        public static IEnumerable<Move> CalculateWPawnMoves(Board board)
        {
            return CalculateWPawnPushes(board).Concat(CalculateWPawnCaptures(board));
        }

        public static IEnumerable<Move> CalculateBPawnMoves(Board board)
        {
            return CalculateBPawnPushes(board).Concat(CalculateBPawnCaptures(board));
        }

        public static IEnumerable<Move> CalculateWPawnPushes(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            UInt64 singlePush = board.WPawns.NorthOne() & board.Empty;
            UInt64 doublePush = singlePush.NorthOne() & board.Empty & Board.Rank4;

            UInt64 promoted = singlePush & Board.Rank8;
            UInt64 nonPromoted = singlePush & ~(promoted);

            for(int i = 0; i < 64; i++)
            {
                if(nonPromoted.NthBitSet(i))
                {
                    moves.Add(new Move(Piece.WPawn, i - 8, i));
                }

                if(doublePush.NthBitSet(i))
                {
                    moves.Add(new Move(Piece.WPawn, i - 16, i));
                }

                if(promoted.NthBitSet((int) i))
                {
                    moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Queen, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Knight, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Rook, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Bishop, SpecialMove.Promotion));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBPawnPushes(Board board) 
        {
            List<Move> moves = new List<Move>(32);

            UInt64 singlePush = board.BPawns.SouthOne() & board.Empty;
            UInt64 doublePush = singlePush.SouthOne() & board.Empty & Board.Rank5;

            UInt64 promoted = singlePush & Board.Rank1;
            UInt64 nonPromoted = singlePush & ~(promoted);

            for(int i = 0; i < 64; i++)
            {
                if(nonPromoted.NthBitSet(i))
                {
                    moves.Add(new Move(Piece.BPawn, i + 8, i));
                }

                if(doublePush.NthBitSet(i))
                {
                    moves.Add(new Move(Piece.BPawn, i + 16, i));
                }

                if(promoted.NthBitSet((int) i))
                {
                    moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Queen, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Knight, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Rook, SpecialMove.Promotion));
                    moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Bishop, SpecialMove.Promotion));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWPawnCaptures(Board board) 
        {
            List<Move> moves = new List<Move>(16);

            UInt64 capturesWest = ((board.WPawns & ~Board.AFile) << 7) & board.BlackPieces;
            UInt64 capturesEast = ((board.WPawns & ~Board.HFile) << 9) & board.BlackPieces;

            for(int i = 0; i < 64; i++)
            {
                if(capturesWest.NthBitSet(i))
                {
                    if(i >= 56)
                    {
                        moves.Add(new Move(Piece.WPawn, i - 7, i, PromotionType.Queen, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 7, i, PromotionType.Knight, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 7, i, PromotionType.Rook, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 7, i, PromotionType.Bishop, SpecialMove.Promotion));
                    }
                    else
                    {
                        moves.Add(new Move(Piece.WPawn, i - 7, i));
                    }
                }

                if(capturesEast.NthBitSet(i))
                {
                    if(i >= 56)
                    {
                        moves.Add(new Move(Piece.WPawn, i - 9, i, PromotionType.Queen, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 9, i, PromotionType.Knight, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 9, i, PromotionType.Rook, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.WPawn, i - 9, i, PromotionType.Bishop, SpecialMove.Promotion));
                    }
                    else
                    {
                        moves.Add(new Move(Piece.WPawn, i - 9, i));
                    }
                }
            }

            // En passant
            if(board.EnPassent != Square.None)
            {
                UInt64 possibleCapturerWest = (1UL << ((int)board.EnPassent - 9)) & board.WPawns & ~Board.HFile;
                UInt64 possibleCapturerEast = (1UL << ((int)board.EnPassent - 7)) & board.WPawns & ~Board.AFile;
               
               if(possibleCapturerWest != 0)
               {
                    UInt64 destination = possibleCapturerWest << 9;
                    if((board.Empty & destination) == destination)
                    {
                        moves.Add(new Move(Piece.WPawn, (int)board.EnPassent - 9, (int)board.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
               }

               if(possibleCapturerEast != 0)
               {
                    UInt64 destination = possibleCapturerEast << 7;
                    if((board.Empty & destination) == destination)
                    {
                        moves.Add(new Move(Piece.WPawn, (int)board.EnPassent - 7, (int)board.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
               }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateBPawnCaptures(Board board) 
        {
            List<Move> moves = new List<Move>(16);

            UInt64 capturesWest = ((board.BPawns & ~Board.AFile) >> 9) & board.WhitePieces;
            UInt64 capturesEast = ((board.BPawns & ~Board.HFile) >> 7) & board.WhitePieces;

            for(int i = 0; i < 64; i++)
            {
                if(capturesWest.NthBitSet(i))
                {
                    if(i < 8)
                    {
                        moves.Add(new Move(Piece.BPawn, i + 9, i, PromotionType.Queen, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 9, i, PromotionType.Knight, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 9, i, PromotionType.Rook, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 9, i, PromotionType.Bishop, SpecialMove.Promotion));
                    }
                    else
                    {
                        moves.Add(new Move(Piece.BPawn, i + 9, i));
                    }
                }

                if(capturesEast.NthBitSet(i))
                {
                    if(i < 8)
                    {
                        moves.Add(new Move(Piece.BPawn, i + 7, i, PromotionType.Queen, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 7, i, PromotionType.Knight, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 7, i, PromotionType.Rook, SpecialMove.Promotion));
                        moves.Add(new Move(Piece.BPawn, i + 7, i, PromotionType.Bishop, SpecialMove.Promotion));
                    }
                    else
                    {
                        moves.Add(new Move(Piece.BPawn, i + 7, i));
                    }
                }
            }

            // En passant
            if (board.EnPassent != Square.None)
            {
                UInt64 possibleCapturerWest = (1UL << ((int)board.EnPassent + 7)) & board.BPawns & ~Board.HFile;
                UInt64 possibleCapturerEast = (1UL << ((int)board.EnPassent + 9)) & board.BPawns & ~Board.AFile;
               
               if(possibleCapturerWest != 0)
               {
                    UInt64 destination = possibleCapturerWest >> 7;
                    if((board.Empty & destination) == destination)
                    {
                        moves.Add(new Move(Piece.BPawn, (int)board.EnPassent + 7, (int)board.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
               }

               if(possibleCapturerEast != 0)
               {
                    UInt64 destination = possibleCapturerEast >> 9;
                    if((board.Empty & destination) == destination)
                    {
                        moves.Add(new Move(Piece.BPawn, (int)board.EnPassent + 9, (int)board.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
               }
            }

            return moves;
        }

        public static UInt64 GetWPawnAttackMap(Board board)
        {
            UInt64 capturesWest = ((board.WPawns & ~Board.AFile) << 7) & ~board.WhitePieces;
            UInt64 capturesEast = ((board.WPawns & ~Board.HFile) << 9) & ~board.WhitePieces;
            return capturesWest | capturesEast;
        }

        public static UInt64 GetBPawnAttackMap(Board board)
        {
            UInt64 capturesWest = ((board.BPawns & ~Board.AFile) >> 9) & ~board.BlackPieces;
            UInt64 capturesEast = ((board.BPawns & ~Board.HFile) >> 7) & ~board.BlackPieces;
            return capturesWest | capturesEast;
        }
    }
}
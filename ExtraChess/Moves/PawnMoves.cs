using ExtraChess.Models;
using System;
using System.Collections.Generic;

namespace ExtraChess.Moves
{
    public static class PawnMoves
    {
        public static List<Move> CalculateWPawnMoves(Board board)
        {
            List<Move> result = new List<Move>(32);
            CalculateWPawnPushes(result, board);
            CalculateWPawnCaptures(result, board);
            return result;
        }

        public static List<Move> CalculateBPawnMoves(Board board)
        {
            List<Move> result = new List<Move>(64);
            CalculateBPawnPushes(result, board);
            CalculateBPawnCaptures(result, board);
            return result;
        }

        public static void CalculateWPawnPushes(List<Move> moves, Board board)
        {
            UInt64 singlePush = board.BoardByPiece[(int)Piece.WPawn].NorthOne() & ~board.Occupied;
            UInt64 doublePush = singlePush.NorthOne() & ~board.Occupied & Constants.Rank4;

            UInt64 promoted = singlePush & Constants.Rank8;
            UInt64 nonPromoted = singlePush & ~promoted;

            foreach (int i in nonPromoted.GetBitsSet())
            {
                moves.Add(new Move(Piece.WPawn, i - 8, i));
            }

            foreach (int i in doublePush.GetBitsSet())
            {
                moves.Add(new Move(Piece.WPawn, i - 16, i));
            }

            foreach (int i in promoted.GetBitsSet())
            {
                moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Queen, SpecialMove.Promotion));
                moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Knight, SpecialMove.Promotion));
                moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Rook, SpecialMove.Promotion));
                moves.Add(new Move(Piece.WPawn, i - 8, i, PromotionType.Bishop, SpecialMove.Promotion));
            }
        }

        public static void CalculateBPawnPushes(List<Move> moves, Board board)
        {
            UInt64 singlePush = board.BoardByPiece[(int)Piece.BPawn].SouthOne() & ~board.Occupied;
            UInt64 doublePush = singlePush.SouthOne() & ~board.Occupied & Constants.Rank5;

            UInt64 promoted = singlePush & Constants.Rank1;
            UInt64 nonPromoted = singlePush & ~promoted;

            foreach (int i in nonPromoted.GetBitsSet())
            {
                moves.Add(new Move(Piece.BPawn, i + 8, i));
            }

            foreach (int i in doublePush.GetBitsSet())
            {
                moves.Add(new Move(Piece.BPawn, i + 16, i));
            }

            foreach (int i in promoted.GetBitsSet())
            {
                moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Queen, SpecialMove.Promotion));
                moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Knight, SpecialMove.Promotion));
                moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Rook, SpecialMove.Promotion));
                moves.Add(new Move(Piece.BPawn, i + 8, i, PromotionType.Bishop, SpecialMove.Promotion));
            }
        }

        public static void CalculateWPawnCaptures(List<Move> moves, Board board)
        {
            UInt64 capturesWest = ((board.BoardByPiece[(int)Piece.WPawn] & ~Constants.AFile) << 7) & board.BoardByColor[(int)Color.Black];
            UInt64 capturesEast = ((board.BoardByPiece[(int)Piece.WPawn] & ~Constants.HFile) << 9) & board.BoardByColor[(int)Color.Black];

            foreach (int i in capturesWest.GetBitsSet())
            {
                if (i >= 56)
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

            foreach (int i in capturesEast.GetBitsSet())
            {
                if (i >= 56)
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

            // En passant
            if (board.State.EnPassent != Square.None)
            {
                UInt64 possibleCapturerWest = (1UL << ((int)board.State.EnPassent - 9)) & board.BoardByPiece[(int)Piece.WPawn] & ~Constants.HFile;
                UInt64 possibleCapturerEast = (1UL << ((int)board.State.EnPassent - 7)) & board.BoardByPiece[(int)Piece.WPawn] & ~Constants.AFile;

                if (possibleCapturerWest != 0)
                {
                    UInt64 destination = possibleCapturerWest << 9;
                    if ((~board.Occupied & destination) == destination)
                    {
                        moves.Add(new Move(Piece.WPawn, (int)board.State.EnPassent - 9, (int)board.State.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
                }

                if (possibleCapturerEast != 0)
                {
                    UInt64 destination = possibleCapturerEast << 7;
                    if ((~board.Occupied & destination) == destination)
                    {
                        moves.Add(new Move(Piece.WPawn, (int)board.State.EnPassent - 7, (int)board.State.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
                }
            }
        }

        public static void CalculateBPawnCaptures(List<Move> moves, Board board)
        {
            UInt64 capturesWest = ((board.BoardByPiece[(int)Piece.BPawn] & ~Constants.AFile) >> 9) & board.BoardByColor[(int)Color.White];
            UInt64 capturesEast = ((board.BoardByPiece[(int)Piece.BPawn] & ~Constants.HFile) >> 7) & board.BoardByColor[(int)Color.White];

            foreach (int i in capturesWest.GetBitsSet())
            {
                if (i < 8)
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

            foreach (int i in capturesEast.GetBitsSet())
            {
                if (i < 8)
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

            // En passant
            if (board.State.EnPassent != Square.None)
            {
                UInt64 possibleCapturerWest = (1UL << ((int)board.State.EnPassent + 7)) & board.BoardByPiece[(int)Piece.BPawn] & ~Constants.HFile;
                UInt64 possibleCapturerEast = (1UL << ((int)board.State.EnPassent + 9)) & board.BoardByPiece[(int)Piece.BPawn] & ~Constants.AFile;

                if (possibleCapturerWest != 0)
                {
                    UInt64 destination = possibleCapturerWest >> 7;
                    if ((~board.Occupied & destination) == destination)
                    {
                        moves.Add(new Move(Piece.BPawn, (int)board.State.EnPassent + 7, (int)board.State.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
                }

                if (possibleCapturerEast != 0)
                {
                    UInt64 destination = possibleCapturerEast >> 9;
                    if ((~board.Occupied & destination) == destination)
                    {
                        moves.Add(new Move(Piece.BPawn, (int)board.State.EnPassent + 9, (int)board.State.EnPassent, specialMove: SpecialMove.EnPassant));
                    }
                }
            }
        }

        public static UInt64 GetPawnAttackMap(Color attackingColor, UInt64 attackingPawns)
        {
            UInt64 capturesWest = (attackingColor == Color.White ? (attackingPawns & ~Constants.AFile) << 7 : (attackingPawns & ~Constants.AFile) >> 9);
            UInt64 capturesEast = (attackingColor == Color.White ? (attackingPawns & ~Constants.HFile) << 9 : (attackingPawns & ~Constants.HFile) >> 7);
            return capturesWest | capturesEast;
        }
    }
}
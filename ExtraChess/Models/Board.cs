using System;
using System.Collections.Generic;
using ExtraChess.Moves;

namespace ExtraChess.Models
{
    public enum Square {
        A1, B1, C1, D1, E1, F1, G1, H1,
        A2, B2, C2, D2, E2, F2, G2, H2,
        A3, B3, C3, D3, E3, F3, G3, H3,
        A4, B4, C4, D4, E4, F4, G4, H4,
        A5, B5, C5, D5, E5, F5, G5, H5,
        A6, B6, C6, D6, E6, F6, G6, H6,
        A7, B7, C7, D7, E7, F7, G7, H7,
        A8, B8, C8, D8, E8, F8, G8, H8,
    }

    public enum InitialSetup
    {
        Regular,
        Kiwipete,
        Position3
    }

    public class Board
    {
        public UInt64 WRooks = 0x81, BRooks = 0x8100000000000000;
        public UInt64 WKnights = 0x42, BKnights = 0x4200000000000000;
        public UInt64 WBishops = 0x24, BBishops = 0x2400000000000000;
        public UInt64 WKing = 0x10, BKing = 0x1000000000000000;
        public UInt64 WQueen = 0x8, BQueen = 0x800000000000000;
        public UInt64 WPawns = Rank2, BPawns = Rank7;

        public UInt64 WhitePieces { get => WRooks | WKnights | WBishops | WKing | WQueen | WPawns; }
        public UInt64 BlackPieces { get => BRooks | BKnights | BBishops | BKing | BQueen | BPawns; }
        public UInt64 Empty { get => ~(WhitePieces | BlackPieces); }
        public UInt64 Occupied { get => WhitePieces | BlackPieces; }

        // Attacks
        public UInt64 WRookAttacks { get => SlidingMoves.GetRookAttackMap(WRooks, Occupied, WhitePieces); set { } }
        public UInt64 BRookAttacks { get => SlidingMoves.GetRookAttackMap(BRooks, Occupied, BlackPieces); set { } }
        public UInt64 WBishopAttacks { get => SlidingMoves.GetBishopAttackMap(WBishops, Occupied, WhitePieces); set { } }
        public UInt64 BBishopAttacks { get => SlidingMoves.GetBishopAttackMap(BBishops, Occupied, BlackPieces); set { } }
        public UInt64 WQueenAttacks { get => SlidingMoves.GetQueenAttackMap(WQueen, Occupied, WhitePieces); set { } }
        public UInt64 BQueenAttacks { get => SlidingMoves.GetQueenAttackMap(BQueen, Occupied, BlackPieces); set { } }
        public UInt64 WPawnAttacks { get => PawnMoves.GetWPawnAttackMap(this); set { } }
        public UInt64 BPawnAttacks { get => PawnMoves.GetBPawnAttackMap(this); set { } }
        public UInt64 WKnightAttacks { get => KnightMoves.GetKnightsAttackMap(WKnights, WhitePieces); set { } }
        public UInt64 BKnightAttacks { get => KnightMoves.GetKnightsAttackMap(BKnights, BlackPieces); set { } }
        public UInt64 WKingAttacks { get => KingMoves.GetKingAttackMap(WKing, WhitePieces); set { } }
        public UInt64 BKingAttacks { get => KingMoves.GetKingAttackMap(BKing, BlackPieces); set { } }

        public UInt64 AllWAttacks { get => WRookAttacks | WBishopAttacks | WQueenAttacks | WPawnAttacks | WKingAttacks | WKnightAttacks; }
        public UInt64 AllBAttacks { get => BRookAttacks | BBishopAttacks | BQueenAttacks | BPawnAttacks | BKingAttacks | BKnightAttacks; }

        /*
        *  Constants
        */

        // Files
        public const UInt64 AFile = 0x0101010101010101;
        public const UInt64 BFile = 0x0202020202020202;
        public const UInt64 CFile = 0x0404040404040404;
        public const UInt64 DFile = 0x0808080808080808;
        public const UInt64 EFile = 0x1010101010101010;
        public const UInt64 FFile = 0x2020202020202020;
        public const UInt64 GFile = 0x4040404040404040;
        public const UInt64 HFile = 0x8080808080808080;

        // Ranks
        public const UInt64 Rank1 = 0x00000000000000FF;
        public const UInt64 Rank2 = 0x000000000000FF00;
        public const UInt64 Rank3 = 0x0000000000FF0000;
        public const UInt64 Rank4 = 0x00000000FF000000;
        public const UInt64 Rank5 = 0x000000FF00000000;
        public const UInt64 Rank6 = 0x0000FF0000000000;
        public const UInt64 Rank7 = 0x00FF000000000000;
        public const UInt64 Rank8 = 0xFF00000000000000;

        // Diagonals
        public const UInt64 A1H8Diagonal = 0x8040201008040201;
        public const UInt64 H1A8Diagonal = 0x0102040810204080;

        // Colored Squares
        public const UInt64 LightSquares = 0x55AA55AA55AA55AA;
        public const UInt64 DarkSquares = 0xAA55AA55AA55AA55;

        // Track castling rights
        public bool WKingMoved { get; private set; } = false;
        public bool WRookRightMoved { get; private set; } = false;
        public bool WRookLeftMoved { get; private set; } = false;
        public bool BKingMoved { get; private set; } = false;
        public bool BRookRightMoved { get; private set; } = false;
        public bool BRookLeftMoved { get; private set; } = false;

        public Board(InitialSetup setup = InitialSetup.Regular)
        {
            switch(setup)
            {
                case InitialSetup.Kiwipete:
                {
                    BPawns = 0UL.SetBit((int)Square.A7).SetBit((int)Square.B4).SetBit((int)Square.C7).SetBit((int)Square.D7).SetBit((int)Square.E6).SetBit((int)Square.F7).SetBit((int)Square.G6).SetBit((int)Square.H3);
                    BBishops = 0UL.SetBit((int)Square.A6).SetBit((int)Square.G7);
                    BKnights = 0UL.SetBit((int)Square.B6).SetBit((int)Square.F6);
                    BQueen = 0UL.SetBit((int)Square.E7);
                    WPawns = 0UL.SetBit((int)Square.A2).SetBit((int)Square.B2).SetBit((int)Square.C2).SetBit((int)Square.D5).SetBit((int)Square.E4).SetBit((int)Square.F2).SetBit((int)Square.G2).SetBit((int)Square.H2);
                    WBishops = 0UL.SetBit((int)Square.D2).SetBit((int)Square.E2);
                    WKnights = 0UL.SetBit((int)Square.C3).SetBit((int)Square.E5);
                    WQueen = 0UL.SetBit((int)Square.F3);
                    break;
                }
                case InitialSetup.Position3:
                {
                    BPawns = 0UL.SetBit((int)Square.C7).SetBit((int)Square.D6).SetBit((int)Square.F4);
                    BKnights = 0;
                    BBishops = 0;
                    BQueen = 0;
                    BRooks = 0UL.SetBit((int)Square.H5);
                    BKing = 0UL.SetBit((int)Square.H4);
                    WPawns = 0UL.SetBit((int)Square.B5).SetBit((int)Square.E2).SetBit((int)Square.G2);
                    WKnights = 0;
                    WBishops = 0;
                    WQueen = 0;
                    WRooks = 0UL.SetBit((int)Square.B4);
                    WKing = 0UL.SetBit((int)Square.A5);
                    break;
                }
                default: break;
            }
        }

        public List<(int position, Piece piece)> GetAllPiecePositions()
        {
            List<(int, Piece)> allPieces = new List<(int, Piece)>();

            for (int i = 0; i < 64; i++)
            {
                if (WRooks.NthBitSet(i)) { allPieces.Add((i, Piece.WRook)); continue; }
                if (WKnights.NthBitSet(i)) { allPieces.Add((i, Piece.WKnight)); continue; }
                if (WBishops.NthBitSet(i)) { allPieces.Add((i, Piece.WBishop)); continue; }
                if (WQueen.NthBitSet(i)) { allPieces.Add((i, Piece.WQueen)); continue; }
                if (WKing.NthBitSet(i)) { allPieces.Add((i, Piece.WKing)); continue; }
                if (WPawns.NthBitSet(i)) { allPieces.Add((i, Piece.WPawn)); continue; }

                if (BRooks.NthBitSet(i)) { allPieces.Add((i, Piece.BRook)); continue; }
                if (BKnights.NthBitSet(i)) { allPieces.Add((i, Piece.BKnight)); continue; }
                if (BBishops.NthBitSet(i)) { allPieces.Add((i, Piece.BBishop)); continue; }
                if (BQueen.NthBitSet(i)) { allPieces.Add((i, Piece.BQueen)); continue; }
                if (BKing.NthBitSet(i)) { allPieces.Add((i, Piece.BKing)); continue; }
                if (BPawns.NthBitSet(i)) { allPieces.Add((i, Piece.BPawn)); continue; }
            }

            return allPieces;
        }

        public void MakeMove(Move move) 
        {
            // Clear to square
            WRooks = WRooks.UnsetBit(move.To);
            BRooks = BRooks.UnsetBit(move.To);
            WKnights = WKnights.UnsetBit(move.To);
            BKnights = BKnights.UnsetBit(move.To);
            WBishops = WBishops.UnsetBit(move.To);
            BBishops = BBishops.UnsetBit(move.To);
            WQueen = WQueen.UnsetBit(move.To);
            BQueen = BQueen.UnsetBit(move.To);
            WKing = WKing.UnsetBit(move.To);
            BKing = BKing.UnsetBit(move.To);
            WPawns = WPawns.UnsetBit(move.To);
            BPawns = BPawns.UnsetBit(move.To);

            // Make move for correct bitboard
            UInt64 from = 1UL << move.From;
            UInt64 to = 1UL << move.To;
            switch(move.Piece)
            {
                case Piece.WRook: 
                {
                    WRooks = (WRooks ^ from) | to; 
                    if(move.From == 0)
                    {
                        WRookLeftMoved = true;
                    } 
                    else if (move.From == 7)
                    {
                        WRookRightMoved = true;
                    }
                    return;
                }
                case Piece.BRook: 
                {
                    BRooks = (BRooks ^ from) | to; 
                    if(move.From == 56)
                    {
                        BRookLeftMoved = true;
                    } 
                    else if (move.From == 63)
                    {
                        BRookRightMoved = true;
                    }
                    return;
                }
                case Piece.WKnight: WKnights = (WKnights ^ from) | to; return;
                case Piece.BKnight: BKnights = (BKnights ^ from) | to; return;
                case Piece.WBishop: WBishops = (WBishops ^ from) | to; return;
                case Piece.BBishop: BBishops = (BBishops ^ from) | to; return;
                case Piece.WQueen: WQueen = (WQueen ^ from) | to; return;
                case Piece.BQueen: BQueen = (BQueen ^ from) | to; return;
                case Piece.WKing: 
                {
                    WKing = (WKing ^ from) | to; 
                    WKingMoved = true;
                    if(move.SpecialMove == SpecialMove.Castling)
                    {
                        if(move.To > move.From)
                        {
                            WRooks = WRooks.UnsetBit((int)Square.H1).SetBit((int)Square.F1);
                        }
                        else
                        {
                            WRooks = WRooks.UnsetBit((int)Square.A1).SetBit((int)Square.D1);
                        }
                    }
                    return;
                }
                case Piece.BKing: 
                {
                    BKing = (BKing ^ from) | to; 
                    BKingMoved = true;
                    if(move.SpecialMove == SpecialMove.Castling)
                    {
                        if(move.To > move.From)
                        {
                            BRooks = BRooks.UnsetBit((int)Square.H8).SetBit((int)Square.F8);
                        }
                        else
                        {
                            BRooks = BRooks.UnsetBit((int)Square.A8).SetBit((int)Square.D8);
                        }
                    }
                    return;
                }
                case Piece.WPawn: 
                {
                    WPawns = (WPawns ^ from) | to; 
                    if(move.SpecialMove == SpecialMove.EnPassant)
                    {
                        BPawns = BPawns.UnsetBit(move.To - 8);
                    }
                    else if (move.SpecialMove == SpecialMove.Promotion)
                    {
                        WPawns = WPawns.UnsetBit(move.To);
                        switch(move.PromotionType)
                        {
                            case PromotionType.Queen: WQueen = WQueen.SetBit(move.To); break;
                            case PromotionType.Knight: WKnights = WKnights.SetBit(move.To); break;
                            case PromotionType.Bishop: WBishops = WBishops.SetBit(move.To); break;
                            case PromotionType.Rook: WRooks = WRooks.SetBit(move.To); break;
                        }
                    }
                    return;
                }
                case Piece.BPawn: 
                {
                    BPawns = (BPawns ^ from) | to; 
                    if(move.SpecialMove == SpecialMove.EnPassant)
                    {
                        WPawns = WPawns.UnsetBit(move.To + 8);
                    }
                    else if (move.SpecialMove == SpecialMove.Promotion)
                    {
                        BPawns = BPawns.UnsetBit(move.To);
                        switch(move.PromotionType)
                        {
                            case PromotionType.Queen: BQueen = BQueen.SetBit(move.To); break;
                            case PromotionType.Knight: BKnights = BKnights.SetBit(move.To); break;
                            case PromotionType.Bishop: BBishops = BBishops.SetBit(move.To); break;
                            case PromotionType.Rook: BRooks = BRooks.SetBit(move.To); break;
                        }
                    }
                    return;
                }
            }
        }

        public Board PreviewMove(Move move)
        {
            Board copy = this.MemberwiseClone() as Board;
            copy.MakeMove(move);
            return copy;
        }

        public bool SquareIsInCheck(Square square, Player playerPossibleInCheck)
        {
            return SquareIsInCheck(0UL.SetBit((int)square), playerPossibleInCheck);
        }

        public bool SquareIsInCheck(UInt64 square, Player playerPossibleInCheck)
        {
            return playerPossibleInCheck == Player.White
                ? (square & AllBAttacks) != 0
                : (square & AllWAttacks) != 0;
        }
    }
}
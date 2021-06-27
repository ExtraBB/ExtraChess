using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ExtraChess.Moves;

namespace ExtraChess.Models
{
    public enum Square
    {
        A1, B1, C1, D1, E1, F1, G1, H1,
        A2, B2, C2, D2, E2, F2, G2, H2,
        A3, B3, C3, D3, E3, F3, G3, H3,
        A4, B4, C4, D4, E4, F4, G4, H4,
        A5, B5, C5, D5, E5, F5, G5, H5,
        A6, B6, C6, D6, E6, F6, G6, H6,
        A7, B7, C7, D7, E7, F7, G7, H7,
        A8, B8, C8, D8, E8, F8, G8, H8, None
    }

    public class Board
    {
        private readonly Regex FenRegex = new(@"\s*^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+)\s([b|w])\s([K|Q|k|q]{1,4})\s(-|[a-h][1-8])\s(\d+\s\d+)$");
        public const string StartPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public UInt64 WRooks, BRooks, WKnights, BKnights, WBishops, BBishops, WKing, BKing, WQueen, BQueen, WPawns, BPawns;

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
        public bool WCanCastleQueenSide { get; private set; } = false;
        public bool WCanCastleKingSide { get; private set; } = false;
        public bool BCanCastleQueenSide { get; private set; } = false;
        public bool BCanCastleKingSide { get; private set; } = false;

        // Track en passent
        public Square EnPassent { get; set; } = Square.None;

        // Track current player
        public Player CurrentPlayer { get; set; } = Player.White;

        // Track move counts
        public int HalfMoves { get; set; } = 0;
        public int FullMoves { get; set; } = 1;

        public Board(string fen = StartPos)
        {
            Magics.Initialize();
            UpdateFromFEN(fen);
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
            // Reset en passent
            EnPassent = Square.None;

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
            switch (move.Piece)
            {
                case Piece.WRook:
                    {
                        WRooks = (WRooks ^ from) | to;
                        if (move.From == 0)
                        {
                            WCanCastleQueenSide = false;
                        }
                        else if (move.From == 7)
                        {
                            WCanCastleKingSide = false;
                        }
                        break;
                    }
                case Piece.BRook:
                    {
                        BRooks = (BRooks ^ from) | to;
                        if (move.From == 56)
                        {
                            BCanCastleQueenSide = false;
                        }
                        else if (move.From == 63)
                        {
                            BCanCastleKingSide = false;
                        }
                        break;
                    }
                case Piece.WKnight: WKnights = (WKnights ^ from) | to; break;
                case Piece.BKnight: BKnights = (BKnights ^ from) | to; break;
                case Piece.WBishop: WBishops = (WBishops ^ from) | to; break;
                case Piece.BBishop: BBishops = (BBishops ^ from) | to; break;
                case Piece.WQueen: WQueen = (WQueen ^ from) | to; break;
                case Piece.BQueen: BQueen = (BQueen ^ from) | to; break;
                case Piece.WKing:
                    {
                        WKing = (WKing ^ from) | to;
                        WCanCastleKingSide = false;
                        WCanCastleQueenSide = false;
                        if (move.SpecialMove == SpecialMove.Castling)
                        {
                            if (move.To > move.From)
                            {
                                WRooks = WRooks.UnsetBit((int)Square.H1).SetBit((int)Square.F1);
                            }
                            else
                            {
                                WRooks = WRooks.UnsetBit((int)Square.A1).SetBit((int)Square.D1);
                            }
                        }
                        break;
                    }
                case Piece.BKing:
                    {
                        BKing = (BKing ^ from) | to;
                        BCanCastleKingSide = false;
                        BCanCastleQueenSide = false;
                        if (move.SpecialMove == SpecialMove.Castling)
                        {
                            if (move.To > move.From)
                            {
                                BRooks = BRooks.UnsetBit((int)Square.H8).SetBit((int)Square.F8);
                            }
                            else
                            {
                                BRooks = BRooks.UnsetBit((int)Square.A8).SetBit((int)Square.D8);
                            }
                        }
                        break;
                    }
                case Piece.WPawn:
                    {
                        if (move.To - move.From == 16)
                        {
                            EnPassent = (Square)move.To - 8;
                        }

                        WPawns = (WPawns ^ from) | to;
                        if (move.SpecialMove == SpecialMove.EnPassant)
                        {
                            BPawns = BPawns.UnsetBit(move.To - 8);
                        }
                        else if (move.SpecialMove == SpecialMove.Promotion)
                        {
                            WPawns = WPawns.UnsetBit(move.To);
                            switch (move.PromotionType)
                            {
                                case PromotionType.Queen: WQueen = WQueen.SetBit(move.To); break;
                                case PromotionType.Knight: WKnights = WKnights.SetBit(move.To); break;
                                case PromotionType.Bishop: WBishops = WBishops.SetBit(move.To); break;
                                case PromotionType.Rook: WRooks = WRooks.SetBit(move.To); break;
                            }
                        }
                        break;
                    }
                case Piece.BPawn:
                    {
                        if (move.From - move.To == 16)
                        {
                            EnPassent = (Square)move.To + 8;
                        }

                        BPawns = (BPawns ^ from) | to;
                        if (move.SpecialMove == SpecialMove.EnPassant)
                        {
                            WPawns = WPawns.UnsetBit(move.To + 8);
                        }
                        else if (move.SpecialMove == SpecialMove.Promotion)
                        {
                            BPawns = BPawns.UnsetBit(move.To);
                            switch (move.PromotionType)
                            {
                                case PromotionType.Queen: BQueen = BQueen.SetBit(move.To); break;
                                case PromotionType.Knight: BKnights = BKnights.SetBit(move.To); break;
                                case PromotionType.Bishop: BBishops = BBishops.SetBit(move.To); break;
                                case PromotionType.Rook: BRooks = BRooks.SetBit(move.To); break;
                            }
                        }
                        break;
                    }
            }

            bool isCapture = (to & (CurrentPlayer == Player.White ? BlackPieces : WhitePieces)) != 0;
            if(move.Piece == Piece.WPawn || move.Piece == Piece.BPawn || isCapture)
            {
                HalfMoves = 0;
            }
            else
            {
                HalfMoves++;
            }

            CurrentPlayer = (Player)(-(int)CurrentPlayer);
            FullMoves++;
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

        private bool UpdateFromFEN(string fen)
        {
            try
            {
                if (!FenRegex.IsMatch(fen))
                {
                    return false;
                }

                string[] split = fen.Split();

                // Update pieces
                string[] pieces = split[0].Split('/');
                for (int rank = 0; rank < 8; rank++)
                {
                    int file = 0;
                    foreach (char nextPiece in pieces[rank])
                    {
                        // Digits are empty
                        if (char.IsDigit(nextPiece))
                        {
                            for (int i = 0; i < (int)nextPiece - 48; i++)
                            {
                                file++;
                            }
                        }
                        else
                        {
                            SetPieceForFENChar(nextPiece, 7 - rank, file);
                            file++;
                        }
                    }
                }

                // Set player
                CurrentPlayer = split[1] == "w" ? Player.White : Player.Black;

                // Set castling rights
                foreach (char c in split[2])
                {
                    if (c == 'K') WCanCastleKingSide = true;
                    if (c == 'Q') WCanCastleQueenSide = true;
                    if (c == 'k') BCanCastleKingSide = true;
                    if (c == 'q') BCanCastleQueenSide = true;
                }

                // Set en passent
                if (split[3] != "-")
                {
                    EnPassent = (Square)Enum.Parse(typeof(Square), split[3].ToUpper());
                }

                HalfMoves = int.Parse(split[4]);
                FullMoves = int.Parse(split[5]);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetPieceForFENChar(char fenChar, int rank, int file)
        {
            switch (fenChar)
            {
                case 'r': BRooks = BRooks.SetBit(rank * 8 + file); return;
                case 'n': BKnights = BKnights.SetBit(rank * 8 + file); return;
                case 'b': BBishops = BBishops.SetBit(rank * 8 + file); return;
                case 'q': BQueen = BQueen.SetBit(rank * 8 + file); return;
                case 'k': BKing = BKing.SetBit(rank * 8 + file); return;
                case 'p': BPawns = BPawns.SetBit(rank * 8 + file); return;
                case 'R': WRooks = WRooks.SetBit(rank * 8 + file); return;
                case 'N': WKnights = WKnights.SetBit(rank * 8 + file); return;
                case 'B': WBishops = WBishops.SetBit(rank * 8 + file); return;
                case 'Q': WQueen = WQueen.SetBit(rank * 8 + file); return;
                case 'K': WKing = WKing.SetBit(rank * 8 + file); return;
                case 'P': WPawns = WPawns.SetBit(rank * 8 + file); return;
                default: throw new ArgumentException("Invalid FEN position char");
            }
        }

        public string GenerateFEN()
        {
            StringBuilder sb = new StringBuilder();

            // Append positions
            for (int rank = 7; rank >= 0; rank--)
            {
                int emptySquares = 0;
                for (int file = 0; file < 8; file++)
                {
                    char nextPiece = GetFENCharForPosition(rank * 8 + file);
                    if(nextPiece == ' ')
                    {
                        emptySquares++;
                    }
                    else
                    {
                        if(emptySquares != 0)
                        {
                            sb.Append(emptySquares);
                            emptySquares = 0;
                        }
                        sb.Append(nextPiece);
                    }
                }
                if (emptySquares != 0)
                {
                    sb.Append(emptySquares);
                }

                if (rank != 0)
                {
                    sb.Append('/');
                }
            }
            sb.Append(' ');

            // Append player
            sb.Append(CurrentPlayer == Player.White ? 'w' : 'b');
            sb.Append(' ');

            // Append castling rights
            if(!WCanCastleKingSide && !WCanCastleQueenSide && !BCanCastleKingSide && !BCanCastleQueenSide)
            {
                sb.Append('-');
            }
            else
            {
                if (WCanCastleKingSide) sb.Append('K');
                if (WCanCastleQueenSide) sb.Append('Q');
                if (BCanCastleKingSide) sb.Append('k');
                if (BCanCastleQueenSide) sb.Append('q');
            }
            sb.Append(' ');

            // Append en passent
            sb.Append(EnPassent != Square.None ? EnPassent.ToString().ToLower() : '-');
            sb.Append(' ');

            // Append move counts
            sb.Append(HalfMoves);
            sb.Append(' ');
            sb.Append(FullMoves);

            return sb.ToString();
        }

        private char GetFENCharForPosition(int position)
        {
            if (BRooks.NthBitSet(position)) return 'r';
            if (WRooks.NthBitSet(position)) return 'R';
            if (BKnights.NthBitSet(position)) return 'n';
            if (WKnights.NthBitSet(position)) return 'N';
            if (BBishops.NthBitSet(position)) return 'b';
            if (WBishops.NthBitSet(position)) return 'B';
            if (BQueen.NthBitSet(position)) return 'q';
            if (WQueen.NthBitSet(position)) return 'Q';
            if (BKing.NthBitSet(position)) return 'k';
            if (WKing.NthBitSet(position)) return 'K';
            if (BPawns.NthBitSet(position)) return 'p';
            if (WPawns.NthBitSet(position)) return 'P';
            return ' ';
        }

        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            for(int rank = 8; rank > 0; rank--)
            {
                sb.AppendLine("+---+---+---+---+---+---+---+---+");
                for (int position = (rank - 1) * 8; position < rank * 8; position++)
                {
                    sb.Append($"| {GetFENCharForPosition(position)} ");
                }
                sb.AppendLine($"| {rank}");
            }
            
            sb.AppendLine("+---+---+---+---+---+---+---+---+");
            sb.AppendLine("  a   b   c   d   e   f   g   h  ");

            Console.Write(sb.ToString());
        }
    }
}
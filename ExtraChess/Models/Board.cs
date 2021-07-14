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
        public Piece[] Pieces = new Piece[64];
        public UInt64[] BoardByPiece = new UInt64[13];
        public UInt64[] BoardByColor = new UInt64[2];
        public UInt64 Occupied;
        public UInt64 Empty { get => ~Occupied; }

        private readonly Regex FenRegex = new(@"^(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\/?){8})\s+(?<SideToMove>b|w)\s+(?<Castling>-|K?Q?k?q?)\s+(?<EnPassant>-|[a-h][3-6])\s+(?<HalfMoveClock>\d+)\s+(?<FullMoveNumber>\d+)\s*$");
        public const string StartPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";


        // Attacks
        public UInt64 WRookAttacks { get => SlidingMoves.GetRookAttackMap(BoardByPiece[(int)Piece.WRook], Occupied, BoardByColor[(int)Color.White]); }
        public UInt64 BRookAttacks { get => SlidingMoves.GetRookAttackMap(BoardByPiece[(int)Piece.BRook], Occupied, BoardByColor[(int)Color.Black]); }
        public UInt64 WBishopAttacks { get => SlidingMoves.GetBishopAttackMap(BoardByPiece[(int)Piece.WBishop], Occupied, BoardByColor[(int)Color.White]); }
        public UInt64 BBishopAttacks { get => SlidingMoves.GetBishopAttackMap(BoardByPiece[(int)Piece.BBishop], Occupied, BoardByColor[(int)Color.Black]); }
        public UInt64 WQueenAttacks { get => SlidingMoves.GetQueenAttackMap(BoardByPiece[(int)Piece.WQueen], Occupied, BoardByColor[(int)Color.White]); }
        public UInt64 BQueenAttacks { get => SlidingMoves.GetQueenAttackMap(BoardByPiece[(int)Piece.BQueen], Occupied, BoardByColor[(int)Color.Black]); }
        public UInt64 WPawnAttacks { get => PawnMoves.GetWPawnAttackMap(this); }
        public UInt64 BPawnAttacks { get => PawnMoves.GetBPawnAttackMap(this); }
        public UInt64 WKnightAttacks { get => KnightMoves.GetKnightsAttackMap(BoardByPiece[(int)Piece.WKnight], BoardByColor[(int)Color.White]); }
        public UInt64 BKnightAttacks { get => KnightMoves.GetKnightsAttackMap(BoardByPiece[(int)Piece.BKnight], BoardByColor[(int)Color.Black]); }
        public UInt64 WKingAttacks { get => KingMoves.GetKingAttackMap(BoardByPiece[(int)Piece.WKing], BoardByColor[(int)Color.White]); }
        public UInt64 BKingAttacks { get => KingMoves.GetKingAttackMap(BoardByPiece[(int)Piece.BKing], BoardByColor[(int)Color.Black]); }

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

        private void MovePiece(int fromSquare, int toSquare)
        {
            UInt64 from = 1UL << fromSquare;
            UInt64 to = 1UL << toSquare;
            UInt64 fromTo = from | to;

            Piece piece = Pieces[fromSquare];
            Color color = piece.ToColor();

            // Make move
            BoardByPiece[(int)piece] = BoardByPiece[(int)piece].ToggleBits(fromTo);
            BoardByColor[(int)color] = BoardByColor[(int)color].ToggleBits(fromTo);
            Occupied = Occupied.ToggleBits(fromTo);
            Pieces[fromSquare] = Piece.None;
            Pieces[toSquare] = piece;
        }

        private void SetPiece(Piece piece, int square)
        {
            Color color = piece.ToColor();

            // Make move
            BoardByPiece[(int)piece] = BoardByPiece[(int)piece].SetBit(square);
            BoardByColor[(int)color] = BoardByColor[(int)color].SetBit(square);
            Occupied = Occupied.SetBit(square);
            Pieces[square] = piece;
        }

        private void UnsetPiece(Piece piece, int square)
        {
            Color color = piece.ToColor();

            // Make move
            BoardByPiece[(int)piece] = BoardByPiece[(int)piece].UnsetBit(square);
            BoardByColor[(int)color] = BoardByColor[(int)color].UnsetBit(square);
            Occupied = Occupied.UnsetBit(square);
            Pieces[square] = Piece.None;
        }

        public void MakeMove(Move move)
        {
            Piece capturedPiece = Pieces[move.To];

            // Reset en passent
            EnPassent = Square.None;

            // Process captures
            if (capturedPiece != Piece.None)
            {
                UnsetPiece(capturedPiece, move.To);

                // Update castling rights for rook captures
                if (move.To == (int)Square.H8)
                {
                    BCanCastleKingSide = false;
                }
                else if (move.To == (int)Square.A8)
                {
                    BCanCastleQueenSide = false;
                }
                else if (move.To == (int)Square.H1)
                {
                    WCanCastleKingSide = false;
                }
                else if (move.To == (int)Square.A1)
                {
                    WCanCastleQueenSide = false;
                }
            }

            // Move piece, update bitboards
            MovePiece(move.From, move.To);

            // Process move specific state
            switch (move.Piece)
            {
                case Piece.WRook:
                    {
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
                case Piece.WKing:
                    {
                        WCanCastleKingSide = false;
                        WCanCastleQueenSide = false;
                        if (move.SpecialMove == SpecialMove.Castling)
                        {
                            if (move.To > move.From)
                            {
                                UnsetPiece(Piece.WRook, (int)Square.H1);
                                SetPiece(Piece.WRook, (int)Square.F1);
                            }
                            else
                            {
                                UnsetPiece(Piece.WRook, (int)Square.A1);
                                SetPiece(Piece.WRook, (int)Square.D1);
                            }
                        }
                        break;
                    }
                case Piece.BKing:
                    {
                        BCanCastleKingSide = false;
                        BCanCastleQueenSide = false;
                        if (move.SpecialMove == SpecialMove.Castling)
                        {
                            if (move.To > move.From)
                            {
                                UnsetPiece(Piece.BRook, (int)Square.H8);
                                SetPiece(Piece.BRook, (int)Square.F8);
                            }
                            else
                            {
                                UnsetPiece(Piece.BRook, (int)Square.A8);
                                SetPiece(Piece.BRook, (int)Square.D8);
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

                        if (move.SpecialMove == SpecialMove.EnPassant)
                        {
                            UnsetPiece(Piece.BPawn, move.To - 8);
                        }
                        else if (move.SpecialMove == SpecialMove.Promotion)
                        {
                            UnsetPiece(Piece.BPawn, move.To);
                            switch (move.PromotionType)
                            {
                                case PromotionType.Queen: SetPiece(Piece.WQueen, move.To); break;
                                case PromotionType.Knight: SetPiece(Piece.WKnight, move.To); break;
                                case PromotionType.Bishop: SetPiece(Piece.WBishop, move.To); break;
                                case PromotionType.Rook: SetPiece(Piece.WRook, move.To); break;
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

                        if (move.SpecialMove == SpecialMove.EnPassant)
                        {
                            UnsetPiece(Piece.WPawn, move.To + 8);
                        }
                        else if (move.SpecialMove == SpecialMove.Promotion)
                        {
                            UnsetPiece(Piece.BPawn, move.To);
                            switch (move.PromotionType)
                            {
                                case PromotionType.Queen: SetPiece(Piece.BQueen, move.To); break;
                                case PromotionType.Knight: SetPiece(Piece.BKnight, move.To); break;
                                case PromotionType.Bishop: SetPiece(Piece.BBishop, move.To); break;
                                case PromotionType.Rook: SetPiece(Piece.BRook, move.To); break;
                            }
                        }
                        break;
                    }
            }

            if(move.Piece == Piece.WPawn || move.Piece == Piece.BPawn || capturedPiece != Piece.None)
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

            copy.Pieces = new Piece[64];
            Array.Copy(Pieces, copy.Pieces, 64);

            copy.BoardByPiece = new UInt64[13];
            Array.Copy(BoardByPiece, copy.BoardByPiece, 13);

            copy.BoardByColor = new UInt64[2];
            Array.Copy(BoardByColor, copy.BoardByColor, 2);

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
            int position = rank * 8 + file;
            switch (fenChar)
            {
                case 'r': SetPiece(Piece.BRook, position); return;
                case 'n': SetPiece(Piece.BKnight, position); return;
                case 'b': SetPiece(Piece.BBishop, position); return;
                case 'q': SetPiece(Piece.BQueen, position); return;
                case 'k': SetPiece(Piece.BKing, position); return;
                case 'p': SetPiece(Piece.BPawn, position); return;
                case 'R': SetPiece(Piece.WRook, position); return;
                case 'N': SetPiece(Piece.WKnight, position); return;
                case 'B': SetPiece(Piece.WBishop, position); return;
                case 'Q': SetPiece(Piece.WQueen, position); return;
                case 'K': SetPiece(Piece.WKing, position); return;
                case 'P': SetPiece(Piece.WPawn, position); return;
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
            switch (Pieces[position])
            {
                case Piece.BRook: return 'r';
                case Piece.WRook: return 'R';
                case Piece.BKnight: return 'n';
                case Piece.WKnight: return 'N';
                case Piece.BBishop: return 'b';
                case Piece.WBishop: return 'B';
                case Piece.BQueen: return 'q';
                case Piece.WQueen: return 'Q';
                case Piece.BKing: return 'k';
                case Piece.WKing: return 'K';
                case Piece.BPawn: return 'p';
                case Piece.WPawn: return 'P';
                default: return ' ';
            }
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
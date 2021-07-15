using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ExtraChess.Moves;

namespace ExtraChess.Models
{
    public class BoardState
    {
        public Move PlayedMove { get; set; }
        public Piece CapturedPiece { get; set; }
        public Square EnPassent { get; set; } = Square.None;
        public Player CurrentPlayer { get; set; } = Player.White;
        public bool WCanCastleQueenSide { get; set; }
        public bool WCanCastleKingSide { get; set; }
        public bool BCanCastleQueenSide { get; set; }
        public bool BCanCastleKingSide { get; set; }
        public int HalfMoves { get; set; }
        public int FullMoves { get; set; }
        public UInt64[] Attacks { get; } = new UInt64[2];
        public UInt64[] Blockers { get; } = new UInt64[2];
        public List<(Piece piece, UInt64 position)>[] Checkers { get; } = new List<(Piece piece, UInt64 position)>[2];
    }

    public class Board
    {
        public Piece[] Pieces = new Piece[64];
        public UInt64[] BoardByPiece = new UInt64[13];
        public UInt64[] BoardByColor = new UInt64[2];
        public UInt64 Occupied;
        public UInt64 Empty { get => ~Occupied; }

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

        // FEN
        private readonly Regex FenRegex = new(@"^(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\/?){8})\s+(?<SideToMove>b|w)\s+(?<Castling>-|K?Q?k?q?)\s+(?<EnPassant>-|[a-h][3-6])\s+(?<HalfMoveClock>\d+)\s+(?<FullMoveNumber>\d+)\s*$");
        public const string StartPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // Track state
        public BoardState State { get; private set; } = new BoardState();
        Stack<BoardState> PreviousStates = new Stack<BoardState>();

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
            // Set variables to restore when unmaking move
            Piece capturedPiece = Pieces[move.To];

            PreviousStates.Push(State);
            State = new BoardState()
            {
                CurrentPlayer = State.CurrentPlayer,
                PlayedMove = move,
                CapturedPiece = capturedPiece,
                EnPassent = State.EnPassent,
                BCanCastleKingSide = State.BCanCastleKingSide,
                BCanCastleQueenSide = State.BCanCastleQueenSide,
                WCanCastleKingSide = State.WCanCastleKingSide,
                WCanCastleQueenSide = State.WCanCastleQueenSide,
                HalfMoves = State.HalfMoves,
                FullMoves = State.FullMoves
            };


            // Reset en passent
            State.EnPassent = Square.None;

            // Process captures
            if (capturedPiece != Piece.None)
            {
                UnsetPiece(capturedPiece, move.To);

                // Update castling rights for rook captures
                if (move.To == (int)Square.H8)
                {
                    State.BCanCastleKingSide = false;
                }
                else if (move.To == (int)Square.A8)
                {
                    State.BCanCastleQueenSide = false;
                }
                else if (move.To == (int)Square.H1)
                {
                    State.WCanCastleKingSide = false;
                }
                else if (move.To == (int)Square.A1)
                {
                    State.WCanCastleQueenSide = false;
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
                            State.WCanCastleQueenSide = false;
                        }
                        else if (move.From == 7)
                        {
                            State.WCanCastleKingSide = false;
                        }
                        break;
                    }
                case Piece.BRook:
                    {
                        if (move.From == 56)
                        {
                            State.BCanCastleQueenSide = false;
                        }
                        else if (move.From == 63)
                        {
                            State.BCanCastleKingSide = false;
                        }
                        break;
                    }
                case Piece.WKing:
                    {
                        State.WCanCastleKingSide = false;
                        State.WCanCastleQueenSide = false;
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
                        State.BCanCastleKingSide = false;
                        State.BCanCastleQueenSide = false;
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
                            State.EnPassent = (Square)move.To - 8;
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
                            State.EnPassent = (Square)move.To + 8;
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
                State.HalfMoves = 0;
            }
            else
            {
                State.HalfMoves++;
            }

            State.CurrentPlayer = (Player)(-(int)State.CurrentPlayer);
            State.FullMoves++;

            UpdateAttacks();
        }

        public void UnmakeMove()
        {
            Move lastMove = State.PlayedMove;
            Piece lastCapture = State.CapturedPiece;
            State = PreviousStates.Pop();

            // Process move specific state
            switch (lastMove.Piece)
            {
                case Piece.WKing:
                    {
                        if (lastMove.SpecialMove == SpecialMove.Castling)
                        {
                            if (lastMove.To > lastMove.From)
                            {
                                UnsetPiece(Piece.WRook, (int)Square.F1);
                                SetPiece(Piece.WRook, (int)Square.H1);
                            }
                            else
                            {
                                UnsetPiece(Piece.WRook, (int)Square.D1);
                                SetPiece(Piece.WRook, (int)Square.A1);
                            }
                        }
                        break;
                    }
                case Piece.BKing:
                    {
                        if (lastMove.SpecialMove == SpecialMove.Castling)
                        {
                            if (lastMove.To > lastMove.From)
                            {
                                UnsetPiece(Piece.BRook, (int)Square.F8);
                                SetPiece(Piece.BRook, (int)Square.H8);
                            }
                            else
                            {
                                UnsetPiece(Piece.BRook, (int)Square.D8);
                                SetPiece(Piece.BRook, (int)Square.A8);
                            }
                        }
                        break;
                    }
                case Piece.WPawn:
                    {
                        if (lastMove.SpecialMove == SpecialMove.EnPassant)
                        {
                            SetPiece(Piece.BPawn, lastMove.To - 8);
                        }
                        else if (lastMove.SpecialMove == SpecialMove.Promotion)
                        {
                            switch (lastMove.PromotionType)
                            {
                                case PromotionType.Queen: UnsetPiece(Piece.WQueen, lastMove.To); break;
                                case PromotionType.Knight: UnsetPiece(Piece.WKnight, lastMove.To); break;
                                case PromotionType.Bishop: UnsetPiece(Piece.WBishop, lastMove.To); break;
                                case PromotionType.Rook: UnsetPiece(Piece.WRook, lastMove.To); break;
                            }

                            SetPiece(Piece.WPawn, lastMove.To);
                        }
                        break;
                    }
                case Piece.BPawn:
                    {
                        if (lastMove.SpecialMove == SpecialMove.EnPassant)
                        {
                            SetPiece(Piece.WPawn, lastMove.To + 8);
                        }
                        else if (lastMove.SpecialMove == SpecialMove.Promotion)
                        {
                            switch (lastMove.PromotionType)
                            {
                                case PromotionType.Queen: UnsetPiece(Piece.BQueen, lastMove.To); break;
                                case PromotionType.Knight: UnsetPiece(Piece.BKnight, lastMove.To); break;
                                case PromotionType.Bishop: UnsetPiece(Piece.BBishop, lastMove.To); break;
                                case PromotionType.Rook: UnsetPiece(Piece.BRook, lastMove.To); break;
                            }

                            SetPiece(Piece.BPawn, lastMove.To);
                        }
                        break;
                    }
            }


            // Move piece back, update bitboards
            MovePiece(lastMove.To, lastMove.From);

            // Restore captured piece
            if (lastCapture != Piece.None)
            {
                SetPiece(lastCapture, lastMove.To);
            }
        }

        public Board Clone()
        {
            Board copy = this.MemberwiseClone() as Board;

            copy.Pieces = new Piece[64];
            Array.Copy(Pieces, copy.Pieces, 64);

            copy.BoardByPiece = new UInt64[13];
            Array.Copy(BoardByPiece, copy.BoardByPiece, 13);

            copy.BoardByColor = new UInt64[2];
            Array.Copy(BoardByColor, copy.BoardByColor, 2);

            return copy;
        }

        public Board PreviewMove(Move move)
        {
            Board copy = Clone();
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

        public bool IsLegalMove(Move move)
        {
            UInt64 to = 1UL << move.To;
            UInt64 from = 1UL << move.From;

            Color opponentColor = State.CurrentPlayer == Player.White ? Color.Black : Color.White;
            Color ownColor = State.CurrentPlayer.ToColor();
            UInt64 attacks = State.Attacks[(int)opponentColor];
            UInt64 kingUnderThreat = BoardByPiece[State.CurrentPlayer == Player.White ? (int)Piece.WKing : (int)Piece.BKing];

            // 1. Check king moves
            if (move.Piece.ToType() == PieceType.King)
            {
                return (attacks & to) == 0;
            }

            // 2. Check evasions
            if (State.Checkers[(int)opponentColor].Count > 0)
            {
                // If multiple checkers, only king move can save the king
                if(State.Checkers[(int)opponentColor].Count > 1)
                {
                    return false;
                }
                else
                {
                    var checker = State.Checkers[(int)opponentColor][0];
                    if (to == checker.position)
                    {
                        // Capture the checker
                        return true;
                    }
                    else if (move.SpecialMove == SpecialMove.EnPassant)
                    {
                        // Capture with en passant
                        if(opponentColor == Color.White && to == checker.position << 8)
                        {
                            return true;
                        }
                        else if (opponentColor == Color.Black && to == checker.position >> 8)
                        {
                            return true;
                        }
                    }
                    else if(checker.piece.ToType() == PieceType.Knight || checker.piece.ToType() == PieceType.Pawn)
                    {
                        // Knights and pawns can't be blocked.
                        return false;
                    }
                    else
                    {
                        // Block the checker
                        return (to & kingUnderThreat.Between(checker.position)) != 0;
                    }
                }
            }

            // 3. Check pins
            if ((State.Blockers[(int)ownColor] & from) != 0)
            {
                if(move.SpecialMove == SpecialMove.EnPassant)
                {
                    // A pinned pawn making en passant never stays on same line 
                    return false;
                }
                if (!kingUnderThreat.IsOnLine(from, to))
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateAttacks()
        {
            if(State.CurrentPlayer == Player.White)
            {
                UInt64 BRookAttacks = SlidingMoves.GetRookAttackMap(BoardByPiece[(int)Piece.BRook], Occupied, BoardByColor[(int)Color.Black]);
                UInt64 BBishopAttacks = SlidingMoves.GetBishopAttackMap(BoardByPiece[(int)Piece.BBishop], Occupied, BoardByColor[(int)Color.Black]);
                UInt64 BQueenAttacks = SlidingMoves.GetQueenAttackMap(BoardByPiece[(int)Piece.BQueen], Occupied, BoardByColor[(int)Color.Black]);
                UInt64 BPawnAttacks = PawnMoves.GetBPawnAttackMap(this);
                UInt64 BKnightAttacks = KnightMoves.GetKnightsAttackMap(BoardByPiece[(int)Piece.BKnight], BoardByColor[(int)Color.Black]);
                UInt64 BKingAttacks = KingMoves.GetKingAttackMap(BoardByPiece[(int)Piece.BKing], BoardByColor[(int)Color.Black]);

                State.Attacks[(int)Color.Black] = BRookAttacks | BBishopAttacks | BQueenAttacks | BPawnAttacks | BKnightAttacks | BKingAttacks;


                // Find checkers
                UInt64 WKing = BoardByPiece[(int)Piece.WKing];
                int WKingPosition = WKing.GetLS1BIndex();

                List<(Piece piece, UInt64 position)> potentialCheckers = new List<(Piece piece, ulong position)>();

                potentialCheckers.Add((Piece.BRook, Constants.Ranks[WKingPosition / 8] & BoardByPiece[(int)Piece.BRook]));
                potentialCheckers.Add((Piece.BRook, Constants.Files[WKingPosition % 8] & BoardByPiece[(int)Piece.BRook]));
                potentialCheckers.Add((Piece.BBishop, Constants.DiagonalsLeft[WKingPosition] & BoardByPiece[(int)Piece.BBishop]));
                potentialCheckers.Add((Piece.BBishop, Constants.DiagonalsRight[WKingPosition] & BoardByPiece[(int)Piece.BBishop]));
                potentialCheckers.Add((Piece.BQueen, Constants.DiagonalsLeft[WKingPosition] & BoardByPiece[(int)Piece.BQueen]));
                potentialCheckers.Add((Piece.BQueen, Constants.DiagonalsRight[WKingPosition] & BoardByPiece[(int)Piece.BQueen]));
                potentialCheckers.Add((Piece.BQueen, Constants.Ranks[WKingPosition / 8] & BoardByPiece[(int)Piece.BQueen]));
                potentialCheckers.Add((Piece.BQueen, Constants.Files[WKingPosition % 8] & BoardByPiece[(int)Piece.BQueen]));

                // TODO: Knights

                // TODO: Pawns

                // TODO: filter checkers for non-zero and non-blocked only


                // TODO: Determine blockers
                State.Blockers[(int)Color.White] = 0;
            }
            else
            {
                // TODO: black
            }
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
                State.CurrentPlayer = split[1] == "w" ? Player.White : Player.Black;

                // Set castling rights
                foreach (char c in split[2])
                {
                    if (c == 'K') State.WCanCastleKingSide = true;
                    if (c == 'Q') State.WCanCastleQueenSide = true;
                    if (c == 'k') State.BCanCastleKingSide = true;
                    if (c == 'q') State.BCanCastleQueenSide = true;
                }

                // Set en passent
                if (split[3] != "-")
                {
                    State.EnPassent = (Square)Enum.Parse(typeof(Square), split[3].ToUpper());
                }

                State.HalfMoves = int.Parse(split[4]);
                State.FullMoves = int.Parse(split[5]);

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
            sb.Append(State.CurrentPlayer == Player.White ? 'w' : 'b');
            sb.Append(' ');

            // Append castling rights
            if(!State.WCanCastleKingSide && !State.WCanCastleQueenSide && !State.BCanCastleKingSide && !State.BCanCastleQueenSide)
            {
                sb.Append('-');
            }
            else
            {
                if (State.WCanCastleKingSide) sb.Append('K');
                if (State.WCanCastleQueenSide) sb.Append('Q');
                if (State.BCanCastleKingSide) sb.Append('k');
                if (State.BCanCastleQueenSide) sb.Append('q');
            }
            sb.Append(' ');

            // Append en passent
            sb.Append(State.EnPassent != Square.None ? State.EnPassent.ToString().ToLower() : '-');
            sb.Append(' ');

            // Append move counts
            sb.Append(State.HalfMoves);
            sb.Append(' ');
            sb.Append(State.FullMoves);

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
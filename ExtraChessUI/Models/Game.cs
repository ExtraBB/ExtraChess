using System.Collections.Generic;
using System.Linq;
using ExtraChess.Models;
using ExtraChess.Moves;

namespace ExtraChessUI.Models
{
    public static class Game
    {
        public delegate void BoardChangedEventHandler(Board board);
        public static event BoardChangedEventHandler BoardChanged;

        public static Board Board { get; private set; }
        public static IEnumerable<Move> PossibleMoves { get; set; } = new List<Move>();
        public static Player Winner { get; set; } = 0;

        public static void Start(string fen = null)
        {
            Clear();
            Board = new Board(fen ?? Board.StartPos);
            RefreshPossibleMoves();
            BoardChanged?.Invoke(Board);
        }

        public static void Clear()
        {
            Board = null;
            PossibleMoves = new List<Move>();
            Winner = 0;
        }

        public static void MakeMove(Move move)
        {
            Board.MakeMove(move);
            RefreshPossibleMoves();
            CheckForEnd();
            BoardChanged?.Invoke(Board);
        }

        private static void RefreshPossibleMoves()
        {
            PossibleMoves = MoveGenerator.GenerateMoves(Board);
        }

        public static void CheckForEnd() 
        {
            if(!PossibleMoves.Any())
            {
                Winner = (Player)(-(int)Board.CurrentPlayer);
            }
        }
    }
}
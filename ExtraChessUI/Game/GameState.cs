using System.Collections.Generic;
using System.Linq;
using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.Moves;

namespace ExtraChessUI.Game
{
    public static class GameState
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

        public static bool TryMakeMove(int from, int to)
        {
            Move move = PossibleMoves.FirstOrDefault(move => move.From == from && move.To == to);
            if(move != null)
            {
                MakeMove(move);
                return true;
            }
            return false;
        }

        public static void MakeMove(Move move)
        {
            Board.MakeMove(move);
            RefreshPossibleMoves();
            CheckForEnd();
            BoardChanged?.Invoke(Board);
        }

        public static IEnumerable<int> GetMovesFromPositionToSquares(int from)
        {
            return PossibleMoves.Where(move => move.From == from).Select(move => move.To);
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
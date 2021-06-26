using System.Collections.Generic;
using System.Linq;
using ExtraChess.Models;
using ExtraChess.Services;

namespace ExtraChessUI.Models
{
    public class Game
    {
        public Board Board { get; private set; }
        public IEnumerable<Move> PossibleMoves { get; set; } = new List<Move>();
        public Player Winner { get; set; } = 0;
        public Move LastMove { get; set; } = null;

        public Game() 
        {
            Board = new Board();
            RefreshPossibleMoves();
        }

        public void MakeMove(Move move) 
        {
            Board.MakeMove(move);
            LastMove = move;
            RefreshPossibleMoves();
            CheckForEnd();
        }

        private void RefreshPossibleMoves()
        {
            PossibleMoves = MoveService.GetAllPossibleMoves(Board, LastMove);
        }

        public void CheckForEnd() 
        {
            if(!PossibleMoves.Any())
            {
                Winner = (Player)(-(int)Board.CurrentPlayer);
            }
        }
    }
}
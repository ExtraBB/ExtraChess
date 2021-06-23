using System;
using System.Collections.Generic;
using ExtraChessUI.Models;
using ExtraChess.Models;
using ExtraChess.Services;

namespace ExtraChessUI.Services
{
    public static class GameService
    {
        public static Game CurrentGame { get; private set; }

        public delegate void BoardChangedEventHandler(Board board);
        public static event BoardChangedEventHandler BoardChanged;

        public static Game StartGame() 
        {
            MagicService.Initialize();
            CurrentGame = new Game();
            BoardChanged?.Invoke(CurrentGame.Board);
            return CurrentGame;
        }

        public static void ClearGame() 
        {
            CurrentGame = null;
        }

        public static void MakeMove(Move move)
        {
            CurrentGame.MakeMove(move);
            BoardChanged?.Invoke(CurrentGame.Board);
        }
    }
}
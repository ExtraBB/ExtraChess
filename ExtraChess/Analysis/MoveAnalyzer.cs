using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.Moves;
using ExtraChess.UCI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class MoveAnalyzer
    {
        public static bool IsLegalMove(Board board, Move move, Player player)
        {
            Board copy = board.PreviewMove(move);
            return player == Player.White
                ? !copy.SquareIsInCheck(copy.WKing, player)
                : !copy.SquareIsInCheck(copy.BKing, player);
        }
    }
}

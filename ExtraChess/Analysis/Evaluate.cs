using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class Evaluate
    {
        public static int EvaluateBoard(Board board)
        {
            int score = 0;

            score += board.WPawns.GetBitsSet().Count() * 100;
            score += board.WBishops.GetBitsSet().Count() * 300;
            score += board.WKnights.GetBitsSet().Count() * 300;
            score += board.WRooks.GetBitsSet().Count() * 500;
            score += board.WQueen.GetBitsSet().Count() * 900;

            score -= board.BPawns.GetBitsSet().Count() * 100;
            score -= board.BBishops.GetBitsSet().Count() * 300;
            score -= board.BKnights.GetBitsSet().Count() * 300;
            score -= board.BRooks.GetBitsSet().Count() * 500;
            score -= board.BQueen.GetBitsSet().Count() * 900;

            return score * (int)board.CurrentPlayer;
        }
    }
}

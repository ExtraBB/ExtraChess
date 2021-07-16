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

            foreach(Piece p in board.Pieces)
            {
                switch(p)
                {
                    case Piece.WPawn: score += 100; break;
                    case Piece.WBishop: score += 300; break;
                    case Piece.WKnight: score += 300; break;
                    case Piece.WRook: score += 500; break;
                    case Piece.WQueen: score += 900; break;
                    case Piece.BPawn: score -= 100; break;
                    case Piece.BBishop: score -= 300; break;
                    case Piece.BKnight: score -= 300; break;
                    case Piece.BRook: score -= 500; break;
                    case Piece.BQueen: score -= 900; break;
                }
            }

            return score * (int)board.State.CurrentPlayer;
        }
    }
}

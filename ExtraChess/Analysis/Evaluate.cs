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

            score += board.BoardByPiece[(int)Piece.WPawn].GetBitsSet().Count() * 100;
            score += board.BoardByPiece[(int)Piece.WBishop].GetBitsSet().Count() * 300;
            score += board.BoardByPiece[(int)Piece.WKnight].GetBitsSet().Count() * 300;
            score += board.BoardByPiece[(int)Piece.WRook].GetBitsSet().Count() * 500;
            score += board.BoardByPiece[(int)Piece.WQueen].GetBitsSet().Count() * 900;

            score -= board.BoardByPiece[(int)Piece.BPawn].GetBitsSet().Count() * 100;
            score -= board.BoardByPiece[(int)Piece.BBishop].GetBitsSet().Count() * 300;
            score -= board.BoardByPiece[(int)Piece.BKnight].GetBitsSet().Count() * 300;
            score -= board.BoardByPiece[(int)Piece.BRook].GetBitsSet().Count() * 500;
            score -= board.BoardByPiece[(int)Piece.BQueen].GetBitsSet().Count() * 900;

            return score * (int)board.State.CurrentPlayer;
        }
    }
}

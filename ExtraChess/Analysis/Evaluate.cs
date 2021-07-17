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

            score += board.PositionsByPiece[Piece.WPawn].Count * 100;
            score += board.PositionsByPiece[Piece.WBishop].Count * 300;
            score += board.PositionsByPiece[Piece.WKnight].Count * 300;
            score += board.PositionsByPiece[Piece.WRook].Count * 500;
            score += board.PositionsByPiece[Piece.WQueen].Count * 900;
            score -= board.PositionsByPiece[Piece.BPawn].Count * 100;
            score -= board.PositionsByPiece[Piece.BBishop].Count * 300;
            score -= board.PositionsByPiece[Piece.BKnight].Count * 300;
            score -= board.PositionsByPiece[Piece.BRook].Count * 500;
            score -= board.PositionsByPiece[Piece.BQueen].Count * 900;

            return score * (int)board.State.CurrentPlayer;
        }
    }
}

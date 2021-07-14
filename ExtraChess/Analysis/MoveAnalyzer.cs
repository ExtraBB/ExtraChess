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
            board.MakeMove(move);
            bool legal = player == Player.White
                ? !board.SquareIsInCheck(board.BoardByPiece[(int)Piece.WKing], player)
                : !board.SquareIsInCheck(board.BoardByPiece[(int)Piece.BKing], player);
            board.UnmakeMove();
            return legal;
        }
    }
}

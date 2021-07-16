using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.UCI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Analysis
{
    public static class Search
    {
        public static bool IsSearching { get; private set; }
        public delegate void BestMoveFoundEventHandler(Move move);
        public static event BestMoveFoundEventHandler BestMoveFound;

        public static async void Start(Board board, long calculateForMillis = long.MaxValue)
        {
            if (IsSearching)
            {
                return;
            }

            try
            {
                IsSearching = true;
                Move move = await Task.Run(() => NegamaxRoot(board, calculateForMillis));
                if (move != null)
                {
                    BestMoveFound?.Invoke(move);
                }
            }
            finally
            {
                IsSearching = false;
            }
        }

        public static void Stop()
        {
            IsSearching = false;
        }

        private static Move NegamaxRoot(Board board, long calculateForMillis = long.MaxValue)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int depth = 0;
            var moves = MoveGenerator.GenerateMoves(board);
            Move bestMove = null;
            int bestScore = -int.MaxValue;

            // Search moves (DFS, iterative deepening)
            while (IsSearching && watch.ElapsedMilliseconds < calculateForMillis)
            {
                int alpha = -int.MaxValue;
                int beta = int.MaxValue;

                // Re-analyze best move first
                if (bestMove != null)
                {
                    board.MakeMove(bestMove);
                    bestScore = -Negamax(board, -beta, -alpha, depth);
                    board.UnmakeMove();
                }

                foreach (Move move in moves)
                {
                    if (move == bestMove)
                    {
                        continue;
                    }

                    board.MakeMove(move);
                    int score = -Negamax(board, -beta, -alpha, depth);
                    board.UnmakeMove();

                    if (score > bestScore)
                    {
                        bestMove = move;
                        bestScore = score;
                    }

                    if (!IsSearching || watch.ElapsedMilliseconds > calculateForMillis)
                    {
                        break;
                    }
                }

                UCISender.SendInfo(depth: depth, pv: bestMove, score: bestScore, time: watch.ElapsedMilliseconds);

                if (!IsSearching || watch.ElapsedMilliseconds > calculateForMillis)
                {
                    break;
                }

                depth++;
            }

            watch.Stop();
            return bestMove;
        }

        private static int Negamax(Board board, int alpha, int beta, int depth)
        {
            if (depth == 0)
            {
                return Evaluate.EvaluateBoard(board);
            }

            var moves = MoveGenerator.GenerateMoves(board);
            if (!moves.Any())
            {
                if (board.SquareIsInCheck(board.State.CurrentPlayer == Player.White ? board.BoardByPiece[(int)Piece.WKing] : board.BoardByPiece[(int)Piece.BKing]))
                {
                    // Checkmate
                    return -int.MaxValue;
                }
                else
                {
                    // Stalemate
                    return 0;
                }
            }

            foreach (Move move in moves)
            {
                board.MakeMove(move, depth > 1);
                int score = -Negamax(board, -beta, -alpha, depth - 1);
                board.UnmakeMove();

                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }
            return alpha;
        }
    }
}

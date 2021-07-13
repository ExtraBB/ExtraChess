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
    public class MoveSet
    {
        public int Score { get; set; }
        public MoveSet PreviousMoveSet { get; set; }
        public Move Move { get; set; }
        public Board Board { get; set; }
    }

    public static class MoveAnalyzer
    {
        public static bool IsAnalyzing { get; private set; }
        public delegate void BestMoveFoundEventHandler(Move move);
        public static event BestMoveFoundEventHandler BestMoveFound;

        public static async void StartAnalysis(Board board, long calculateForMillis = long.MaxValue)
        {
            if (IsAnalyzing)
            {
                return;
            }

            try
            {
                IsAnalyzing = true;

                await Task.Run(() =>
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    int depth = 0;

                    var moves = MoveGenerator.GenerateMoves(board);
                    int bestScore = -int.MaxValue;
                    Move bestMove = null;

                    // Search moves (DFS, iterative deepening)
                    while (IsAnalyzing && watch.ElapsedMilliseconds < calculateForMillis)
                    {
                        // Re-analyze best move first
                        if(bestMove != null)
                        {
                            bestScore = -Negamax(board.PreviewMove(bestMove), depth);
                        }

                        foreach (Move move in moves)
                        {
                            if(move == bestMove)
                            {
                                continue;
                            }

                            int score = -Negamax(board.PreviewMove(move), depth);

                            if (score > bestScore)
                            {
                                bestMove = move;
                                bestScore = score;
                            }

                            if (!IsAnalyzing || watch.ElapsedMilliseconds > calculateForMillis)
                            {
                                break;
                            }
                        }

                        UCISender.SendInfo(depth: depth, pv: bestMove, score: bestScore, time: watch.ElapsedMilliseconds);

                        if (!IsAnalyzing || watch.ElapsedMilliseconds > calculateForMillis)
                        {
                            break;
                        }

                        depth++;
                    }

                    BestMoveFound?.Invoke(bestMove);
                    watch.Stop();
                });
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        private static int Negamax(Board board, int depth)
        {
            if(depth == 0)
            { 
                return Evaluate(board);
            }

            int max = -int.MaxValue;

            foreach(Move move in MoveGenerator.GenerateMoves(board))
            {
                int score = -Negamax(board.PreviewMove(move), depth - 1);
                if (score > max)
                {
                    max = score;
                }
            }
            return max;
        }

        public static void StopAnalysis()
        {
            IsAnalyzing = false;
        }

        public static bool IsLegalMove(Board board, Move move, Player player)
        {
            Board copy = board.PreviewMove(move);
            return player == Player.White
                ? !copy.SquareIsInCheck(copy.WKing, player)
                : !copy.SquareIsInCheck(copy.BKing, player);
        }

        public static int Evaluate(Board board)
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

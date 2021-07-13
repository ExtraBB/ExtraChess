using ExtraChess.Generators;
using ExtraChess.Models;
using ExtraChess.Moves;
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
        private static Random random = new Random();

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

                    int depth = 1;

                    var moves = MoveGenerator.GenerateMoves(board);
                    Dictionary<Move, double> result = new Dictionary<Move, double>();

                    // Search moves (DFS, iterative deepening)
                    while (IsAnalyzing && watch.ElapsedMilliseconds < calculateForMillis)
                    {
                        foreach (Move move in moves)
                        {
                            result[move] = FindMoveScoreDFS(board.PreviewMove(move), board.CurrentPlayer, depth);

                            if (!IsAnalyzing || watch.ElapsedMilliseconds > calculateForMillis)
                            {
                                break;
                            }
                        }
                        depth++;
                    }

                    BestMoveFound?.Invoke(result.MaxBy(kvp => kvp.Value).Key);
                    watch.Stop();
                });
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        private static double FindMoveScoreDFS(Board board, Player player, int depth)
        {
            if(depth == 0)
            { 
                return GetBoardScore(board, player);
            }

            var moves = MoveGenerator.GenerateMoves(board);

            // Mate
            if(!moves.Any())
            {
                return board.CurrentPlayer == player ? -40 : 40;
            }
            return moves.Average(m => FindMoveScoreDFS(board.PreviewMove(m), player, depth - 1));
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

        public static int GetBoardScore(Board board, Player player)
        {
            int score = 0;
            int playerFactor = (int)player;

            score += board.WPawns.GetBitsSet().Count() * playerFactor;
            score += board.WBishops.GetBitsSet().Count() * 3 * playerFactor;
            score += board.WKnights.GetBitsSet().Count() * 3 * playerFactor;
            score += board.WRooks.GetBitsSet().Count() * 5 * playerFactor;
            score += board.WQueen.GetBitsSet().Count() * 9 * playerFactor;

            score += board.BPawns.GetBitsSet().Count() * -playerFactor;
            score += board.BBishops.GetBitsSet().Count() * 3 * -playerFactor;
            score += board.BKnights.GetBitsSet().Count() * 3 * -playerFactor;
            score += board.BRooks.GetBitsSet().Count() * 5 * -playerFactor;
            score += board.BQueen.GetBitsSet().Count() * 9 * -playerFactor;

            return score;
        }
    }
}

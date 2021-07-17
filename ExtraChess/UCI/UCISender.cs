using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.UCI
{
    public static class UCISender
    {
        public static void SendBestMove(Move move)
        {
            Console.WriteLine($"bestmove {move.ToUCIMove()}");
        }

        public static void SendPerft(List<(Move, ulong)> results, long elapsed)
        {
            ulong total = 0;
            foreach ((Move move, ulong perft) result in results)
            {
                Console.WriteLine($"{result.move.ToUCIMove()}: {result.perft}");
                total += result.perft;
            }
            Console.WriteLine($"Nodes searched: {total} ({elapsed}ms)");
        }

        public static void SendEngineInfo()
        {
            Version engineVersion = Assembly.GetEntryAssembly().GetName().Version;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"id name ExtraChess v{engineVersion.Major}.{engineVersion.Minor}");
            stringBuilder.AppendLine($"id author Bruno Carvalhal");

            // TODO: Print supported options here

            stringBuilder.AppendLine("uciok");

            Console.Write(stringBuilder.ToString());
        }

        public static void SendHelp()
        {
            Console.Write(File.ReadAllText("Resources/uci_help_text_en.txt"));
        }

        public static void SendInfo(int depth = -1, long time = -1, Move pv = null, int score = -1, long nodes = -1)
        {
            string info = "info ";
            if (depth != -1)
            {
                info += $"depth {depth} ";
            }

            if (pv != null)
            {
                info += $"move {pv} ";
            }

            if (score != -1)
            {
                info += $"score {score} ";
            }

            if (time != -1)
            {
                info += $"time {time} ";
            }

            if (nodes != -1)
            {
                info += $"nodes {nodes} ";
            }

            Console.WriteLine(info);
        }
    }
}

using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess.Generators
{
    public static class BoardGenerator
    {
        public static bool IsGenerating { get; private set; }

        public static Board GenerateBoardFromUCIPosition(params string[] uciArgs)
        {
            try
            {
                IsGenerating = true;
                Board board = new Board();

                int movesArgumentIndex = Array.IndexOf(uciArgs, "moves");

                if (uciArgs[0] == "fen")
                {
                    string fen = movesArgumentIndex != -1
                        ? string.Join(' ', uciArgs.Skip(1).Take(movesArgumentIndex - 1))
                        : string.Join(' ', uciArgs.Skip(1));
                    board = new Board(fen);
                }
                else if (uciArgs[0] != "startpos")
                {
                    throw new UnknownCommandException(string.Join(' ', uciArgs));
                }

                if (movesArgumentIndex != -1)
                {
                    foreach (string uciMove in uciArgs.Skip(movesArgumentIndex + 1))
                    {
                        IEnumerable<Move> generatedMoves = MoveGenerator.GenerateMoves(board);
                        Move move = Move.UCIMoveToMove(generatedMoves, uciMove);
                        if (move == null)
                        {
                            break;
                        }
                        board.MakeMove(move);
                    }
                }

                return board;
            }
            finally
            {
                IsGenerating = false;
            }
        }
    }
}

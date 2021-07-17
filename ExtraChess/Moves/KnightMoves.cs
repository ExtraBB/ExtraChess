using ExtraChess.Generators;
using ExtraChess.Models;
using System;
using System.Collections.Generic;

namespace ExtraChess.Moves
{
    public static class KnightMoves
    {
        private static UInt64[] KnightMovesLookupTable;

        public static List<Move> CalculateBKnightMoves(Board board)
        {
            if (KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }

            List<Move> moves = new List<Move>(16);

            foreach (int i in board.PositionsByPiece[Piece.BKnight])
            {
                UInt64 attacks = KnightMovesLookupTable[i] & ~board.BoardByColor[(int)Color.Black];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BKnight));
            }

            return moves;
        }

        public static List<Move> CalculateWKnightMoves(Board board)
        {
            if (KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }

            List<Move> moves = new List<Move>(16);

            foreach (int i in board.PositionsByPiece[Piece.WKnight])
            {
                UInt64 attacks = KnightMovesLookupTable[i] & ~board.BoardByColor[(int)Color.White];
                moves.AddRange(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WKnight));
            }

            return moves;
        }

        public static UInt64 GetKnightsAttackMap(List<int> knights, UInt64 ownPieces)
        {
            if (KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }

            UInt64 allAttacks = 0;
            foreach (int i in knights)
            {
                allAttacks |= KnightMovesLookupTable[i] & ~ownPieces;
            }
            return allAttacks;
        }

        public static List<(int, UInt64)> GetSplitKnightsAttackMap(List<int> knights)
        {
            if (KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }

            List<(int, UInt64)> allAttacks = new List<(int, UInt64)>();
            foreach (int i in knights)
            {
                allAttacks.Add((i, KnightMovesLookupTable[i]));
            }
            return allAttacks;
        }

        private static void GenerateKnightMoves()
        {
            KnightMovesLookupTable = new UInt64[64];

            UInt64 spot_1_clip = ~(Constants.AFile | Constants.BFile);
            UInt64 spot_2_clip = ~Constants.AFile;
            UInt64 spot_3_clip = ~Constants.HFile;
            UInt64 spot_4_clip = ~(Constants.HFile | Constants.GFile);

            UInt64 spot_5_clip = ~(Constants.HFile | Constants.GFile);
            UInt64 spot_6_clip = ~Constants.HFile;
            UInt64 spot_7_clip = ~Constants.AFile;
            UInt64 spot_8_clip = ~(Constants.AFile | Constants.BFile);

            for (int i = 0; i < 64; i++)
            {
                UInt64 knight = 1UL << i;

                UInt64 spot_1 = (knight & spot_1_clip) << 6;
                UInt64 spot_2 = (knight & spot_2_clip) << 15;
                UInt64 spot_3 = (knight & spot_3_clip) << 17;
                UInt64 spot_4 = (knight & spot_4_clip) << 10;

                UInt64 spot_5 = (knight & spot_5_clip) >> 6;
                UInt64 spot_6 = (knight & spot_6_clip) >> 15;
                UInt64 spot_7 = (knight & spot_7_clip) >> 17;
                UInt64 spot_8 = (knight & spot_8_clip) >> 10;

                KnightMovesLookupTable[i] = spot_1 | spot_2 | spot_3 | spot_4 | spot_5 | spot_6 | spot_7 | spot_8;
            }
        }
    }
}
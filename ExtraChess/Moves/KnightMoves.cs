using ExtraChess.Generators;
using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraChess.Moves
{
    public static class KnightMoves 
    {
        private static UInt64[] KnightMovesLookupTable;

        public static IEnumerable<Move> CalculateBKnightMoves(Board board)
        {
            if(KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }
            
            IEnumerable<Move> moves = new List<Move>(16);

            for(int i = 0; i < 64; i++)
            {
                if(board.BKnights.NthBitSet(i))
                {
                    UInt64 attacks = KnightMovesLookupTable[i] & ~board.BlackPieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.BKnight));
                }
            }

            return moves;
        }

        public static IEnumerable<Move> CalculateWKnightMoves(Board board)
        {
            if(KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }
            
            IEnumerable<Move> moves = new List<Move>(16);

            for(int i = 0; i < 64; i++)
            {
                if(board.WKnights.NthBitSet(i))
                {
                    UInt64 attacks = KnightMovesLookupTable[i] & ~board.WhitePieces;
                    moves = moves.Concat(MoveGenerator.GenerateMovesFromBitboard(attacks, i, Piece.WKnight));
                }
            }

            return moves;
        }

        public static UInt64 GetKnightsAttackMap(UInt64 knights, UInt64 ownPieces)
        {
            if(KnightMovesLookupTable == null)
            {
                GenerateKnightMoves();
            }

            UInt64 allAttacks = 0;
            for(int i = 0; i < 64; i++)
            {
                if(knights.NthBitSet(i))
                {
                    allAttacks |= KnightMovesLookupTable[i] & ~ownPieces;
                }
            }
            return allAttacks;
        }

        private static void GenerateKnightMoves()
        {
            KnightMovesLookupTable = new UInt64[64];

            UInt64 spot_1_clip = ~(Board.AFile | Board.BFile);
            UInt64 spot_2_clip = ~Board.AFile;
            UInt64 spot_3_clip = ~Board.HFile;
            UInt64 spot_4_clip = ~(Board.HFile | Board.GFile);

            UInt64 spot_5_clip = ~(Board.HFile | Board.GFile);
            UInt64 spot_6_clip = ~Board.HFile;
            UInt64 spot_7_clip = ~Board.AFile;
            UInt64 spot_8_clip = ~(Board.AFile | Board.BFile);

            for(int i = 0; i < 64; i++)
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
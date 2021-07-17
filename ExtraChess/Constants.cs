using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    public static class Constants
    {
        public const UInt64 AllSquares = ~0UL;

        // Ranks
        public static readonly UInt64[] Ranks = new UInt64[]
        {
            0x00000000000000FF,
            0x000000000000FF00,
            0x0000000000FF0000,
            0x00000000FF000000,
            0x000000FF00000000,
            0x0000FF0000000000,
            0x00FF000000000000,
            0xFF00000000000000
        };

        public static UInt64 Rank1 = Ranks[0];
        public static UInt64 Rank2 = Ranks[1];
        public static UInt64 Rank3 = Ranks[2];
        public static UInt64 Rank4 = Ranks[3];
        public static UInt64 Rank5 = Ranks[4];
        public static UInt64 Rank6 = Ranks[5];
        public static UInt64 Rank7 = Ranks[6];
        public static UInt64 Rank8 = Ranks[7];

        // Files
        public static readonly UInt64[] Files = new UInt64[]
        {
           0x0101010101010101,
           0x0202020202020202,
           0x0404040404040404,
           0x0808080808080808,
           0x1010101010101010,
           0x2020202020202020,
           0x4040404040404040,
           0x8080808080808080
        };

        public static UInt64 AFile = Files[0];
        public static UInt64 BFile = Files[1];
        public static UInt64 CFile = Files[2];
        public static UInt64 DFile = Files[3];
        public static UInt64 EFile = Files[4];
        public static UInt64 FFile = Files[5];
        public static UInt64 GFile = Files[6];
        public static UInt64 HFile = Files[7];

        public static readonly UInt64[] DiagonalsRight = new UInt64[]
        {
            0x80,
            0x8040,
            0x804020,
            0x80402010,
            0x8040201008,
            0x804020100804,
            0x80402010080402,
            0x8040201008040201,
            0x4020100804020100,
            0x2010080402010000,
            0x1008040201000000,
            0x0804020100000000,
            0x0402010000000000,
            0x0201000000000000,
            0x0100000000000000,
        };

        // rank + (7 - file)
        public static readonly UInt64[] DiagonalsRightBySquare = new UInt64[]
        {
             DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5], DiagonalsRight[4], DiagonalsRight[3], DiagonalsRight[2], DiagonalsRight[1], DiagonalsRight[0],
             DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5], DiagonalsRight[4], DiagonalsRight[3], DiagonalsRight[2], DiagonalsRight[1],
             DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5], DiagonalsRight[4], DiagonalsRight[3], DiagonalsRight[2],
             DiagonalsRight[10], DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5], DiagonalsRight[4], DiagonalsRight[3],
             DiagonalsRight[11], DiagonalsRight[10], DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5], DiagonalsRight[4],
             DiagonalsRight[12], DiagonalsRight[11], DiagonalsRight[10], DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6], DiagonalsRight[5],
             DiagonalsRight[13], DiagonalsRight[12], DiagonalsRight[11], DiagonalsRight[10], DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7], DiagonalsRight[6],
             DiagonalsRight[14], DiagonalsRight[13], DiagonalsRight[12], DiagonalsRight[11], DiagonalsRight[10], DiagonalsRight[9], DiagonalsRight[8], DiagonalsRight[7],
        };

        public static readonly UInt64[] DiagonalsLeft = new UInt64[]
        {
            0x01,
            0x0102,
            0x010204,
            0x01020408,
            0x0102040810,
            0x010204081020,
            0x01020408102040,
            0x0102040810204080,
            0x0204081020408000,
            0x0408102040800000,
            0x0810204080000000,
            0x1020408000000000,
            0x2040800000000000,
            0x4080000000000000,
            0x8000000000000000,
        };

        // rank + file
        public static readonly UInt64[] DiagonalsLeftBySquare = new UInt64[]
        {
             DiagonalsLeft[0], DiagonalsLeft[1], DiagonalsLeft[2], DiagonalsLeft[3], DiagonalsLeft[4], DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7],
             DiagonalsLeft[1], DiagonalsLeft[2], DiagonalsLeft[3], DiagonalsLeft[4], DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8],
             DiagonalsLeft[2], DiagonalsLeft[3], DiagonalsLeft[4], DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9],
             DiagonalsLeft[3], DiagonalsLeft[4], DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9], DiagonalsLeft[10],
             DiagonalsLeft[4], DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9], DiagonalsLeft[10], DiagonalsLeft[11],
             DiagonalsLeft[5], DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9], DiagonalsLeft[10], DiagonalsLeft[11], DiagonalsLeft[12],
             DiagonalsLeft[6], DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9], DiagonalsLeft[10], DiagonalsLeft[11], DiagonalsLeft[12], DiagonalsLeft[13],
             DiagonalsLeft[7], DiagonalsLeft[8], DiagonalsLeft[9], DiagonalsLeft[10], DiagonalsLeft[11], DiagonalsLeft[12], DiagonalsLeft[13], DiagonalsLeft[14],
        };

        private static UInt64[,] linesByCombination = null;
        public static UInt64[,] LinesByCombination
        {
            get
            {
                if(linesByCombination == null)
                {
                    UInt64[,] result = new UInt64[64, 64];
                    for (int start = 0; start < 64; start++)
                    {
                        for (int end = 0; end < 64; end++)
                        {
                            // Horizontal line
                            int startRank = start / 8;
                            int endRank = end / 8;

                            if (startRank == endRank)
                            {
                                result[start, end] = Constants.Ranks[startRank];
                                continue;
                            }

                            // Vertical line
                            int startFile = start % 8;
                            int endFile = end % 8;

                            if (startFile == endFile)
                            {
                                result[start, end] = Constants.Files[startFile];
                                continue;
                            }

                            // Diagonals
                            if (DiagonalsLeft[startRank + startFile] == DiagonalsLeft[endRank + endFile])
                            {
                                result[start, end] = DiagonalsLeft[startRank + startFile];
                                continue;
                            }

                            if (DiagonalsRight[startRank + (7 - startFile)] == DiagonalsRight[endRank + (7 - endFile)])
                            {
                                result[start, end] = DiagonalsRight[startRank + (7 - startFile)];
                                continue;
                            }
                            result[start, end] = 0;
                        }
                    }
                    linesByCombination = result;
                }

                return linesByCombination;
            }
        }
    }
}

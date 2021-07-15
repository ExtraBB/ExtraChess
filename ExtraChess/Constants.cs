using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    public static class Constants
    {
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

        // Diagonals
        public static UInt64 A1H8Diagonal = DiagonalsRight[7];
        public static UInt64 H1A8Diagonal = DiagonalsLeft[7];

        // Colored Squares
        public const UInt64 LightSquares = 0x55AA55AA55AA55AA;
        public const UInt64 DarkSquares = 0xAA55AA55AA55AA55;
    }
}

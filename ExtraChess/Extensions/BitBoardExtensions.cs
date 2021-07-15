using System;
using System.Collections.Generic;
using System.Text;

namespace ExtraChess 
{
    public static class BitBoardExtensions
    {
        public static UInt64 NorthOne(this UInt64 val)
        {
            return val << 8;
        }

        public static UInt64 WestOne(this UInt64 val)
        {
            return val >> 1;
        }

        public static UInt64 SouthOne(this UInt64 val)
        {
            return val >> 8;
        }

        public static UInt64 EastOne(this UInt64 val)
        {
            return val << 1;
        }

        public static bool NthBitSet(this UInt64 value, int n)
        {
            return value.GetBit(n) != 0;
        }

        public static IEnumerable<int> GetBitsSet(this UInt64 value)
        {
            int counter = 0;
            while(value > 0)
            {
                if((value & 1UL) == 1UL)
                {
                    yield return counter;
                }
                value >>= 1;
                counter++;
            }
        }

        public static UInt64 SetBit(this UInt64 value, int n)
        {
            return value | (1UL << n);
        }

        public static UInt64 UnsetBit(this UInt64 value, int n)
        {
            return value & ~(1UL << n);
        }

        public static UInt64 GetBit(this UInt64 value, int n)
        {
            return value & (1UL << n);
        }

        public static UInt64 ToggleBits(this UInt64 value, UInt64 bits)
        {
            return value ^ bits;
        }

        public static int BitCount(this UInt64 value)
        {
            int count = 0;
            
            // pop bits until bitboard is empty
            while (value != 0)
            {
                count++;
                // consecutively reset least significant 1st bit
                value &= value - 1;
            }
            
            return count;
        }

        public static int GetLS1BIndex(this UInt64 value)
        {
            if (value != 0)
            {
                // convert trailing zeros before LS1B to ones and count them
                return ((value & (~value + 1)) - 1).BitCount();
            }
            else
            {
                return -1;
            }
        }

        // Assumes only 1 bit is set
        public static bool IsOnLine(this UInt64 square, UInt64 lineStart, UInt64 lineEnd)
        {
            int start = lineStart.GetLS1BIndex();
            int end = lineEnd.GetLS1BIndex();

            // Horizontal line
            int startRank = start / 8;
            int endRank = end / 8;
            
            if(startRank == endRank)
            {
                return (Constants.Ranks[startRank] & square) != 0;
            }

            // Vertical line
            int startFile = start % 8;
            int endFile = end % 8;

            if (startFile == endFile)
            {
                return (Constants.Files[startFile] & square) != 0;
            }

            // Diagonal

            return false;
        }

        public static void Print(this UInt64 value)
        {
            StringBuilder sb = new StringBuilder();
            // loop over board ranks
            for (int rank = 7; rank >= 0; rank--)
            {
                // loop over board files
                for (int file = 0; file < 8; file++)
                {
                    // init board square
                    int square = rank * 8 + file;
                    
                    // print ranks
                    if (file == 0)
                    {
                        sb.Append($" {rank + 1} ");
                    }
                    
                    // print bit indexed by board square
                    int bitset = value.NthBitSet(square) ? 1 : 0;
                    sb.Append($" {bitset}");
                }
                
                sb.Append("\n");
            }
            
            // print files
            sb.Append("    a b c d e f g h\n");
            
            // print value as decimal
            sb.Append($"bitboard: {value}");

            Console.WriteLine(sb.ToString());
        }
    }
}
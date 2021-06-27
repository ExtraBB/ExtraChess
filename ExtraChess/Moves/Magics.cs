using System;
using System.Collections.Generic;
using ExtraChess.Models;

namespace ExtraChess.Moves
{
    public struct SMagic {
        public UInt64 Mask { get; set; }  // to mask relevant squares of both lines (no outer squares)
        public UInt64 Magic { get; set; } // magic 64-bit factor
    };

    public static class Magics
    {
        private static bool initialized = false;
        private static UInt64[,] BishopAttacks = new UInt64[64, 512];
        private static UInt64[,] RookAttacks  = new UInt64[64, 4096];
        private static UInt64[] BishopMasks = new UInt64[64];
        private static UInt64[] RookMasks = new UInt64[64];
        public static readonly UInt64[] RookMagics = new UInt64[64] 
        {
            0x8a80104000800020UL,
            0x140002000100040UL,
            0x2801880a0017001UL,
            0x100081001000420UL,
            0x200020010080420UL,
            0x3001c0002010008UL,
            0x8480008002000100UL,
            0x2080088004402900UL,
            0x800098204000UL,
            0x2024401000200040UL,
            0x100802000801000UL,
            0x120800800801000UL,
            0x208808088000400UL,
            0x2802200800400UL,
            0x2200800100020080UL,
            0x801000060821100UL,
            0x80044006422000UL,
            0x100808020004000UL,
            0x12108a0010204200UL,
            0x140848010000802UL,
            0x481828014002800UL,
            0x8094004002004100UL,
            0x4010040010010802UL,
            0x20008806104UL,
            0x100400080208000UL,
            0x2040002120081000UL,
            0x21200680100081UL,
            0x20100080080080UL,
            0x2000a00200410UL,
            0x20080800400UL,
            0x80088400100102UL,
            0x80004600042881UL,
            0x4040008040800020UL,
            0x440003000200801UL,
            0x4200011004500UL,
            0x188020010100100UL,
            0x14800401802800UL,
            0x2080040080800200UL,
            0x124080204001001UL,
            0x200046502000484UL,
            0x480400080088020UL,
            0x1000422010034000UL,
            0x30200100110040UL,
            0x100021010009UL,
            0x2002080100110004UL,
            0x202008004008002UL,
            0x20020004010100UL,
            0x2048440040820001UL,
            0x101002200408200UL,
            0x40802000401080UL,
            0x4008142004410100UL,
            0x2060820c0120200UL,
            0x1001004080100UL,
            0x20c020080040080UL,
            0x2935610830022400UL,
            0x44440041009200UL,
            0x280001040802101UL,
            0x2100190040002085UL,
            0x80c0084100102001UL,
            0x4024081001000421UL,
            0x20030a0244872UL,
            0x12001008414402UL,
            0x2006104900a0804UL,
            0x1004081002402UL,
        };

        public static readonly UInt64[] BishopMagics = new UInt64[64] 
        {
            0x40040844404084UL,
            0x2004208a004208UL,
            0x10190041080202UL,
            0x108060845042010UL,
            0x581104180800210UL,
            0x2112080446200010UL,
            0x1080820820060210UL,
            0x3c0808410220200UL,
            0x4050404440404UL,
            0x21001420088UL,
            0x24d0080801082102UL,
            0x1020a0a020400UL,
            0x40308200402UL,
            0x4011002100800UL,
            0x401484104104005UL,
            0x801010402020200UL,
            0x400210c3880100UL,
            0x404022024108200UL,
            0x810018200204102UL,
            0x4002801a02003UL,
            0x85040820080400UL,
            0x810102c808880400UL,
            0xe900410884800UL,
            0x8002020480840102UL,
            0x220200865090201UL,
            0x2010100a02021202UL,
            0x152048408022401UL,
            0x20080002081110UL,
            0x4001001021004000UL,
            0x800040400a011002UL,
            0xe4004081011002UL,
            0x1c004001012080UL,
            0x8004200962a00220UL,
            0x8422100208500202UL,
            0x2000402200300c08UL,
            0x8646020080080080UL,
            0x80020a0200100808UL,
            0x2010004880111000UL,
            0x623000a080011400UL,
            0x42008c0340209202UL,
            0x209188240001000UL,
            0x400408a884001800UL,
            0x110400a6080400UL,
            0x1840060a44020800UL,
            0x90080104000041UL,
            0x201011000808101UL,
            0x1a2208080504f080UL,
            0x8012020600211212UL,
            0x500861011240000UL,
            0x180806108200800UL,
            0x4000020e01040044UL,
            0x300000261044000aUL,
            0x802241102020002UL,
            0x20906061210001UL,
            0x5a84841004010310UL,
            0x4010801011c04UL,
            0xa010109502200UL,
            0x4a02012000UL,
            0x500201010098b028UL,
            0x8040002811040900UL,
            0x28000010020204UL,
            0x6000020202d0240UL,
            0x8918844842082200UL,
            0x4010011029020020UL,
        };

        public static readonly int[] RookRelevantBits = new int [] {
            12, 11, 11, 11, 11, 11, 11, 12,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            11, 10, 10, 10, 10, 10, 10, 11,
            12, 11, 11, 11, 11, 11, 11, 12
        };

        public static readonly int[] BishopRelevantBits = new int [] {
            6, 5, 5, 5, 5, 5, 5, 6,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            6, 5, 5, 5, 5, 5, 5, 6
        };

        public static UInt64 GetBishopAttacks(UInt64 occupancy, int position) 
        {
            occupancy &= BishopMasks[position];
            occupancy *= BishopMagics[position];
            occupancy >>= 64 - BishopRelevantBits[position];
            return BishopAttacks[position, occupancy];
        }

        public static UInt64 GetRookAttacks(UInt64 occupancy, int position) 
        {
            occupancy &= RookMasks[position];
            occupancy *= RookMagics[position];
            occupancy >>= 64 - RookRelevantBits[position];
            return RookAttacks[position, occupancy];
        }
        
        public static void Initialize()
        {
            if(!initialized)
            {
                InitializeSlidersAttacks(true);
                InitializeSlidersAttacks(false);
                initialized = true;
            }
        }

        private static void InitializeSlidersAttacks(bool isBishop)
        {
            for (int square = 0; square < 64; square++)
            {
                // init bishop & rook masks
                BishopMasks[square] = MaskBishopAttacks(square);
                RookMasks[square] = MaskRookAttacks(square);
                
                // init current mask
                UInt64 mask = isBishop ? MaskBishopAttacks(square) : MaskRookAttacks(square);
                
                // count attack mask bits
                int bitCount = mask.BitCount();
                
                // occupancy variations count
                int occupancyVariations = 1 << bitCount;
                
                // loop over occupancy variations
                for (int count = 0; count < occupancyVariations; count++)
                {
                    // bishop
                    if (isBishop)
                    {
                        // init occupancies, magic index & attacks
                        UInt64 occupancy = SetOccupancy(count, bitCount, mask);
                        UInt64 magicIndex = occupancy * BishopMagics[square] >> 64 - BishopRelevantBits[square];
                        BishopAttacks[square, magicIndex] = GenerateBishopAttacks(square, occupancy);                
                    }
                    
                    // rook
                    else
                    {
                        // init occupancies, magic index & attacks
                        UInt64 occupancy = SetOccupancy(count, bitCount, mask);
                        UInt64 magicIndex = occupancy * RookMagics[square] >> 64 - RookRelevantBits[square];
                        RookAttacks[square, magicIndex] = GenerateRookAttacks(square, occupancy);                
                    }
                }
            }
        }

        private static UInt64 MaskBishopAttacks(int square)
        {
            // attack bitboard
            UInt64 attacks = 0UL;
            
            // init files & ranks
            int f, r;
            
            // init target files & ranks
            int tr = square / 8;
            int tf = square % 8;
            
            // generate attacks
            for (r = tr + 1, f = tf + 1; r <= 6 && f <= 6; r++, f++) attacks |= (1UL << (r * 8 + f));
            for (r = tr + 1, f = tf - 1; r <= 6 && f >= 1; r++, f--) attacks |= (1UL << (r * 8 + f));
            for (r = tr - 1, f = tf + 1; r >= 1 && f <= 6; r--, f++) attacks |= (1UL << (r * 8 + f));
            for (r = tr - 1, f = tf - 1; r >= 1 && f >= 1; r--, f--) attacks |= (1UL << (r * 8 + f));
            
            // return attack map for bishop on a given square
            return attacks;
        }

        private static UInt64 MaskRookAttacks(int square)
        {
            // attacks bitboard
            UInt64 attacks = 0UL;
            
            // init files & ranks
            int f, r;
            
            // init target files & ranks
            int tr = square / 8;
            int tf = square % 8;
            
            // generate attacks
            for (r = tr + 1; r <= 6; r++) attacks |= (1UL << (r * 8 + tf));
            for (r = tr - 1; r >= 1; r--) attacks |= (1UL << (r * 8 + tf));
            for (f = tf + 1; f <= 6; f++) attacks |= (1UL << (tr * 8 + f));
            for (f = tf - 1; f >= 1; f--) attacks |= (1UL << (tr * 8 + f));
            
            // return attack map for bishop on a given square
            return attacks;
        }

        private static UInt64 GenerateBishopAttacks(int square, UInt64 block)
        {
            // attack bitboard
            UInt64 attacks = 0UL;
            
            // init files & ranks
            int f, r;
            
            // init target files & ranks
            int tr = square / 8;
            int tf = square % 8;
            
            // generate attacks
            for (r = tr + 1, f = tf + 1; r <= 7 && f <= 7; r++, f++)
            {
                attacks |= (1UL << (r * 8 + f));
                if ((block & (1UL << (r * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            for (r = tr + 1, f = tf - 1; r <= 7 && f >= 0; r++, f--)
            {
                attacks |= (1UL << (r * 8 + f));
                if ((block & (1UL << (r * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            for (r = tr - 1, f = tf + 1; r >= 0 && f <= 7; r--, f++)
            {
                attacks |= (1UL << (r * 8 + f));
                if ((block & (1UL << (r * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            for (r = tr - 1, f = tf - 1; r >= 0 && f >= 0; r--, f--)
            {
                attacks |= (1UL << (r * 8 + f));
                if ((block & (1UL << (r * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            // return attack map for bishop on a given square
            return attacks;
        }

        private static UInt64 GenerateRookAttacks(int square, UInt64 block)
        {
            // attacks bitboard
            UInt64 attacks = 0UL;
            
            // init files & ranks
            int f, r;
            
            // init target files & ranks
            int tr = square / 8;
            int tf = square % 8;
            
            // generate attacks
            for (r = tr + 1; r <= 7; r++)
            {
                attacks |= (1UL << (r * 8 + tf));
                if ((block & (1UL << (r * 8 + tf))) != 0)
                {
                    break;
                }
            }
            
            for (r = tr - 1; r >= 0; r--)
            {
                attacks |= (1UL << (r * 8 + tf));
                if ((block & (1UL << (r * 8 + tf))) != 0)
                {
                    break;
                }
            }
            
            for (f = tf + 1; f <= 7; f++)
            {
                attacks |= (1UL << (tr * 8 + f));
                if ((block & (1UL << (tr * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            for (f = tf - 1; f >= 0; f--)
            {
                attacks |= (1UL << (tr * 8 + f));
                if ((block & (1UL << (tr * 8 + f))) != 0)
                {
                    break;
                }
            }
            
            // return attack map for bishop on a given square
            return attacks;
        }

        private static UInt64 SetOccupancy(int index, int bitsInMask, UInt64 attackMask)
        {
            // occupancy map
            UInt64 occupancy = 0UL;
            
            // loop over the range of bits within attack mask
            for (int count = 0; count < bitsInMask; count++)
            {
                // get LS1B index of attacks mask
                int square = attackMask.GetLS1BIndex();
                
                // pop LS1B in attack map
                attackMask = attackMask.UnsetBit(square);
                
                // make sure occupancy is on board
                if ((index & (1 << count)) != 0)
                {
                    // populate occupancy map
                    occupancy |= (1UL << square);
                }
            }
            
            // return occupancy map
            return occupancy;
        }
    }
}
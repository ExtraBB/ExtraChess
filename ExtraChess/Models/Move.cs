using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraChess.Models
{
    public enum PromotionType : uint
    {
        None = 0,
        Queen = 1,
        Knight = 2,
        Rook = 3,
        Bishop = 4
    }

    public enum SpecialMove : uint
    {
        None = 0,
        Promotion = 1,
        EnPassant = 2,
        Castling = 3
    }

    public class Move
    {
        public Piece Piece { get; set; }
        public int To { get; set; }
        public int From { get; set; }
        public PromotionType PromotionType { get; set; }
        public SpecialMove SpecialMove { get; set; }

        public Move(Piece piece, int from, int to, PromotionType promotion = PromotionType.None, SpecialMove specialMove = SpecialMove.None)
        {
            Piece = piece;
            From = from;
            To = to;
            PromotionType = promotion;
            SpecialMove = specialMove;
        }

        public Move(Piece piece, Square from, Square to, PromotionType promotion = PromotionType.None, SpecialMove specialMove = SpecialMove.None)
        {
            Piece = piece;
            From = (int)from;
            To = (int)to;
            PromotionType = promotion;
            SpecialMove = specialMove;
        }

        public static Move UCIMoveToMove(IEnumerable<Move> moves, string uciMove)
        {
            try
            {
                int from = (int)Enum.Parse(typeof(Square), string.Concat(uciMove.Take(2)).ToUpper());
                int to = (int)Enum.Parse(typeof(Square), string.Concat(uciMove.Skip(2).Take(2)).ToUpper());
                PromotionType promotionType = PromotionType.None;

                if (uciMove.Length == 5)
                {
                    switch (uciMove[4])
                    {
                        case 'q': promotionType = PromotionType.Queen; break;
                        case 'r': promotionType = PromotionType.Rook; break;
                        case 'n': promotionType = PromotionType.Knight; break;
                        case 'b': promotionType = PromotionType.Bishop; break;
                    }
                }

                return moves.FirstOrDefault(m => m.From == from && m.To == to && m.PromotionType == promotionType);
            }
            catch
            {
                throw new InvalidMoveException();
            }
        }

        public string ToUCIMove()
        {
            string result = "";

            result += ((Square)From).ToString().ToLower();
            result += ((Square)To).ToString().ToLower();

            switch (PromotionType)
            {
                case PromotionType.Queen: result += 'q'; break;
                case PromotionType.Rook: result += 'r'; break;
                case PromotionType.Knight: result += 'n'; break;
                case PromotionType.Bishop: result += 'b'; break;
            }

            return result;
        }
    }
}
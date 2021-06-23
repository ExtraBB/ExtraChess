namespace ExtraChess.Models
{
    public enum PromotionType : uint
    {
        Queen = 0,
        Knight = 1,
        Rook = 2,
        Bishop = 3
    }

    public enum SpecialMove : uint
    {
        None = 0,
        Promotion = 1,
        EnPassant = 2,
        Castling = 3
    }

    public class OldMove
    {
        public int From { get; set; }
        public int To { get; set; }
        public Piece Player { get; set; }
    }

    public class Move
    {
        public Piece Piece { get; set; }
        public int To { get; set; }
        public int From { get; set; }
        public PromotionType PromotionType { get; set; }
        public SpecialMove SpecialMove { get; set; }

        public Move(Piece piece, int from, int to, PromotionType promotion = PromotionType.Queen, SpecialMove specialMove = SpecialMove.None)
        {
            Piece = piece;
            From = from;
            To = to;
            PromotionType = promotion;
            SpecialMove = specialMove;
        }

        public Move(Piece piece, Square from, Square to, PromotionType promotion = PromotionType.Queen, SpecialMove specialMove = SpecialMove.None)
        {
            Piece = piece;
            From = (int)from;
            To = (int)to;
            PromotionType = promotion;
            SpecialMove = specialMove;
        }
    }
}
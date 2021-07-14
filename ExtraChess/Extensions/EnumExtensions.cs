using ExtraChess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtraChess
{
    internal static class EnumExtensions
    {
        public static string Description<Enum>(this Enum source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        public static Color ToColor(this Piece piece)
        {
            switch(piece)
            {
                case Piece.WPawn: return Color.White;
                case Piece.WQueen: return Color.White;
                case Piece.WKing: return Color.White;
                case Piece.WBishop: return Color.White;
                case Piece.WRook: return Color.White;
                case Piece.WKnight: return Color.White;
                default: return Color.Black;
            }
        }
    }
}

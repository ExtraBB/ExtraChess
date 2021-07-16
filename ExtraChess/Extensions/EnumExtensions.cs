﻿using ExtraChess.Models;
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

        public static PieceType ToType(this Piece piece)
        {
            switch (piece)
            {
                case Piece.WPawn: return PieceType.Pawn;
                case Piece.WQueen: return PieceType.Queen;
                case Piece.WKing: return PieceType.King;
                case Piece.WBishop: return PieceType.Bishop;
                case Piece.WRook: return PieceType.Rook;
                case Piece.WKnight: return PieceType.Knight;
                case Piece.BPawn: return PieceType.Pawn;
                case Piece.BQueen: return PieceType.Queen;
                case Piece.BKing: return PieceType.King;
                case Piece.BBishop: return PieceType.Bishop;
                case Piece.BRook: return PieceType.Rook;
                case Piece.BKnight: return PieceType.Knight;
                default: throw new InvalidOperationException();
            }
        }

        public static Color ToColor(this Player player)
        {
            switch (player)
            {
                case Player.White: return Color.White;
                case Player.Black: return Color.Black;
                default: throw new InvalidOperationException();
            }
        }
    }
}

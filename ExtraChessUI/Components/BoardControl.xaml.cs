using ExtraChess.Models;
using ExtraChessUI.Models;
using ExtraChessUI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ExtraChessUI.Components
{
    public class BoardItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int position;
        public int Position { get => position; set { position = value; RefreshBackground(); NotifyPropertyChanged(); } }

        private bool selected;
        public bool Selected { get => selected; set { selected = value; RefreshBackground(); NotifyPropertyChanged(); } }

        private Visibility moveVisibility = Visibility.Collapsed;
        public Visibility MoveVisibility { get => moveVisibility; set { moveVisibility = value; RefreshBackground(); NotifyPropertyChanged(); } }

        private string background;
        public string Background { get => background; set { background = value; NotifyPropertyChanged(); } }

        private BitmapImage image;
        public BitmapImage Image { get => image; set { image = value; NotifyPropertyChanged(); } }

        private void RefreshBackground()
        {
            string darkSquare = Selected ? "#bbca2b" : "#769656";
            string lightSquare = Selected ? "#f6f668" : "#eeeed2";

            Background = Position / 8 % 2 == 0
                    ? (Position % 2 == 0 ? darkSquare : lightSquare)
                    : (Position % 2 == 0 ? lightSquare : darkSquare);
        }
    }

    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        public TrulyObservableCollection<BoardItem> BoardItems { get; set; } = new TrulyObservableCollection<BoardItem>();
        private BoardItem SelectedItem;

        // Images
        private static Dictionary<Piece, BitmapImage> PieceImages = new Dictionary<Piece, BitmapImage>()
        {
            { Piece.WRook, new BitmapImage(new Uri("/Resources/wrook.png", UriKind.Relative)) },
            { Piece.WKnight, new BitmapImage(new Uri("/Resources/wknight.png", UriKind.Relative)) },
            { Piece.WBishop, new BitmapImage(new Uri("/Resources/wbishop.png", UriKind.Relative)) },
            { Piece.WQueen, new BitmapImage(new Uri("/Resources/wqueen.png", UriKind.Relative)) },
            { Piece.WKing, new BitmapImage(new Uri("/Resources/wking.png", UriKind.Relative)) },
            { Piece.WPawn, new BitmapImage(new Uri("/Resources/wpawn.png", UriKind.Relative)) },

            { Piece.BRook, new BitmapImage(new Uri("/Resources/brook.png", UriKind.Relative)) },
            { Piece.BKnight, new BitmapImage(new Uri("/Resources/bknight.png", UriKind.Relative)) },
            { Piece.BBishop, new BitmapImage(new Uri("/Resources/bbishop.png", UriKind.Relative)) },
            { Piece.BQueen, new BitmapImage(new Uri("/Resources/bqueen.png", UriKind.Relative)) },
            { Piece.BKing, new BitmapImage(new Uri("/Resources/bking.png", UriKind.Relative)) },
            { Piece.BPawn, new BitmapImage(new Uri("/Resources/bpawn.png", UriKind.Relative)) }
        };

        public BoardControl()
        {
            DataContext = this;
            InitializeComponent();
            ResetBoard();
        }

        public void ResetBoard()
        {
            BoardItems.Clear();
            for (int i = 63; i >= 0; i--)
            {
                BoardItems.Add(new BoardItem() { Position = i });
            }
        }

        private void Square_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Update selected item
            BoardItem newItem = (sender as Grid).Tag as BoardItem;
            newItem.Selected = !newItem.Selected;
            newItem.MoveVisibility = Visibility.Collapsed;

            // Check if move was made
            if (SelectedItem != null)
            {
                Move move = Game.PossibleMoves.FirstOrDefault(move => move.From == SelectedItem.Position && move.To == newItem.Position);
                if(move != null)
                {
                    Game.MakeMove(move);
                    SelectedItem = null;
                    return;
                }
            }

            // Update selected item
            if(SelectedItem != null)
            {
                SelectedItem.Selected = false;
            }
            SelectedItem = newItem.Selected ? newItem : null;

            // Update move indicators
            var movesFromPosition = newItem.Selected ? Game.PossibleMoves.Where(move => move.From == newItem.Position) : Enumerable.Empty<Move>();
            foreach (var item in BoardItems)
            {
                item.MoveVisibility = movesFromPosition.Any(move => move.To == item.Position) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void UpdateFromBoard(Board board)
        {
            ResetBoard();

            foreach ((int position, Piece piece) in board.GetAllPiecePositions())
            {
                BoardItems[63 - position].Image = PieceImages[piece];
            }
        }
    }
}

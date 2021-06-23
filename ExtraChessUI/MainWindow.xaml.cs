using ExtraChess.Models;
using ExtraChess.Services;
using ExtraChessUI.Services;
using System.Diagnostics;
using System.Windows;

namespace ExtraChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GameService.BoardChanged += GameService_BoardChanged;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            GameService.StartGame();
        }

        private void Perft_Click(object sender, RoutedEventArgs e)
        {
            if(GameService.CurrentGame == null)
            {
                GameService.StartGame();
            }

            AnalysisOutput.Text = $"Starting Perft for depth {(int)PerftDepth.Value}\n";

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 1; i <= (int)PerftDepth.Value; i++)
            {
                ulong perftValue = MoveService.Perft(GameService.CurrentGame.Board, i, GameService.CurrentGame.LastMove, GameService.CurrentGame.CurrentPlayer);
                AnalysisOutput.Text += $"Perft for depth {i} is {perftValue} ({watch.ElapsedMilliseconds} ms)\n";
                watch.Restart();
            }
            watch.Stop();
        }

        private void GameService_BoardChanged(Board board)
        {
            BoardControl.UpdateFromBoard(board);
        }
    }
}

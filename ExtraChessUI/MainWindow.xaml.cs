using ExtraChess.Models;
using ExtraChess.Services;
using ExtraChessUI.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

            // Settings
            int perftSetting = (int)PerftDepth.Value;
            Board board = GetBoardForSetting();
            Move lastMove = board == GameService.CurrentGame.Board ? GameService.CurrentGame.LastMove : null;
            Player player = board == GameService.CurrentGame.Board ? GameService.CurrentGame.CurrentPlayer : Player.White;

            Task.Run(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                ulong perftValue = MoveService.PerftConcurrent(board, perftSetting, lastMove, player);

                this.Dispatcher.Invoke(() =>
                {
                    AnalysisOutput.Text += $"Perft for depth {PerftDepth.Value} is {perftValue} ({watch.ElapsedMilliseconds} ms)\n";
                });

                watch.Stop();
            });
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            if (GameService.CurrentGame == null)
            {
                GameService.StartGame();
            }

            AnalysisOutput.Text = $"Starting PerftDivide for depth {(int)PerftDepth.Value}\n";

            // Settings
            int perftSetting = (int)PerftDepth.Value;
            Board board = GetBoardForSetting();
            Move lastMove = board == GameService.CurrentGame.Board ? GameService.CurrentGame.LastMove : null;
            Player player = board == GameService.CurrentGame.Board ? GameService.CurrentGame.CurrentPlayer : Player.White;

            var moves = MoveService.GetAllPossibleMoves(board, player, lastMove).OrderBy(move => ((Square)move.From).ToString());

            Task.Run(() =>
            {
                Parallel.ForEach(moves, m =>
                {
                    var newBoard = board.PreviewMove(m);
                    var perft = MoveService.Perft(newBoard, perftSetting, m, player: (Player)(-(int)player));
                    if (perft != 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            AnalysisOutput.Text += $"{(Square)m.From}{(Square)m.To}: {perft}\n".ToLower();
                        });
                    }
                });
            });
        }

        private Board GetBoardForSetting()
        {
            switch((PerftBoardSetting.SelectedItem as ComboBoxItem).Name)
            {
                case "CurrentBoard": return GameService.CurrentGame.Board;
                case "RegularBoard": return new Board(InitialSetup.Regular);
                case "Kiwipete": return new Board(InitialSetup.Kiwipete);
                case "Position3": return new Board(InitialSetup.Position3);
            }
            return GameService.CurrentGame.Board;
        }

        private void GameService_BoardChanged(Board board)
        {
            BoardControl.UpdateFromBoard(board);
        }
    }
}

using ExtraChess.Models;
using ExtraChess.Services;
using ExtraChessUI.Models;
using ExtraChessUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public ObservableCollection<string> EngineOptions { get; set; } = new ObservableCollection<string>();
        public string FEN { get; set; } = "FEN string here";
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            GameService.BoardChanged += GameService_BoardChanged;
            InitializeEngineComboBox();
        }

        private void InitializeEngineComboBox()
        {
            foreach (string path in Directory.GetFiles("."))
            {
                if (Path.GetExtension(path).ToLower().Equals(".exe"))
                {
                    string name = Path.GetFileNameWithoutExtension(path);
                    if (name != Assembly.GetExecutingAssembly().GetName().Name)
                    {
                        EngineOptions.Add(Path.GetFileNameWithoutExtension(path));
                    }
                }
            }

            if (EngineOptions.Count > 0)
            {
                EngineComboBox.SelectedItem = EngineOptions[0];
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            GameService.StartGame();
        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            GameService.ClearGame();
        }

        private void LoadFen_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            GameService.ClearGame();

            // TODO: Load FEN
        }

        private void Perft_Click(object sender, RoutedEventArgs e)
        {
            if (GameService.CurrentGame == null)
            {
                GameService.StartGame();
            }

            AnalysisOutput.Text = $"Starting Perft for depth {(int)PerftDepth.Value}\n";

            // Settings
            int perftSetting = (int)PerftDepth.Value;
            Board board = GetBoardForSetting();
            ExtraChess.Models.Move lastMove = board == GameService.CurrentGame.Board ? GameService.CurrentGame.LastMove : null;
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
            ExtraChess.Models.Move lastMove = board == GameService.CurrentGame.Board ? GameService.CurrentGame.LastMove : null;
            Player player = board == GameService.CurrentGame.Board ? GameService.CurrentGame.CurrentPlayer : Player.White;

            var moves = MoveService.GetAllPossibleMoves(board, player, lastMove).OrderBy(move => ((ExtraChess.Models.Square)move.From).ToString());

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
            switch ((PerftBoardSetting.SelectedItem as ComboBoxItem).Name)
            {
                case "CurrentBoard": return GameService.CurrentGame.Board;
                case "RegularBoard": return new Board();
                case "Kiwipete": return new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");
                case "Position3": return new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -");
                case "Position4": return new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
                case "Position5": return new Board("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
                case "Position6": return new Board("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            }
            return GameService.CurrentGame.Board;
        }

        private void GameService_BoardChanged(Board board)
        {
            BoardControl.UpdateFromBoard(board);
        }

        private void EngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO Load engine
        }
    }
}

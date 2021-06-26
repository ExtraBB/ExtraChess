using ExtraChess.Models;
using ExtraChess.Moves;
using ExtraChessUI.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExtraChessUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<string> EngineOptions { get; set; } = new ObservableCollection<string>();

        private string fen;
        public string FEN { get => fen; set { fen = value; NotifyPropertyChanged(); } }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel = new MainWindowViewModel();
        public MainWindow()
        {
            DataContext = viewModel;
            InitializeComponent();
            Game.BoardChanged += GameService_BoardChanged;
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
                        viewModel.EngineOptions.Add(Path.GetFileNameWithoutExtension(path));
                    }
                }
            }

            if (viewModel.EngineOptions.Count > 0)
            {
                EngineComboBox.SelectedItem = viewModel.EngineOptions[0];
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            Game.Start();
        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            Game.Clear();
        }

        private void LoadFen_Click(object sender, RoutedEventArgs e)
        {
            BoardControl.ResetBoard();
            Game.Start(viewModel.FEN);
        }

        private void Perft_Click(object sender, RoutedEventArgs e)
        {
            AnalysisOutput.Text = $"Starting Perft for depth {(int)PerftDepth.Value}\n";

            // Settings
            int perftSetting = (int)PerftDepth.Value;
            Board board = GetBoardForSetting();

            Task.Run(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                ulong perftValue = MoveGenerator.PerftConcurrent(board, perftSetting);

                this.Dispatcher.Invoke(() =>
                {
                    AnalysisOutput.Text += $"Perft for depth {PerftDepth.Value} is {perftValue} ({watch.ElapsedMilliseconds} ms)\n";
                });

                watch.Stop();
            });
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            AnalysisOutput.Text = $"Starting PerftDivide for depth {(int)PerftDepth.Value}\n";

            // Settings
            int perftSetting = (int)PerftDepth.Value;
            Board board = GetBoardForSetting();

            var moves = MoveGenerator.GenerateMoves(board).OrderBy(move => ((Square)move.From).ToString());

            Task.Run(() =>
            {
                Parallel.ForEach(moves, m =>
                {
                    var perft = MoveGenerator.Perft(board.PreviewMove(m), perftSetting);
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
                case "CurrentBoard": return Game.Board;
                case "RegularBoard": return new Board();
                case "Kiwipete": return new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");
                case "Position3": return new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - -");
                case "Position4": return new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
                case "Position5": return new Board("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
                case "Position6": return new Board("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            }
            return Game.Board;
        }

        private void GameService_BoardChanged(Board board)
        {
            BoardControl.UpdateFromBoard(board);
            viewModel.FEN = board.GenerateFEN();
        }

        private void EngineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO Load engine
        }
    }
}

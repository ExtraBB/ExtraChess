using ExtraChess.Analysis;
using ExtraChess.Models;
using ExtraChess.Moves;
using ExtraChessUI.Models;
using ExtraChessUI.Utils;
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
        private const string SELECT_ENGINE_PLACEHOLDER_TEXT = "Select Engine";
        private MainWindowViewModel viewModel = new MainWindowViewModel();

        private EngineProcess CurrentEngine = null;

        public MainWindow()
        {
            DataContext = viewModel;
            InitializeComponent();
            Game.BoardChanged += GameService_BoardChanged;
            InitializeEngineComboBox();
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (CurrentEngine != null)
            {
                CurrentEngine.OutputReceived -= CurrentEngine_OutputReceived;
                CurrentEngine.Dispose();
            }
        }

        private void InitializeEngineComboBox()
        {
            viewModel.EngineOptions.Add(SELECT_ENGINE_PLACEHOLDER_TEXT);

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

            EngineComboBox.SelectedItem = viewModel.EngineOptions.First();
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
            CurrentEngine?.SendMessage("position fen " + viewModel.FEN);
        }

        private void Perft_Click(object sender, RoutedEventArgs e)
        {
            Board board = GetBoardForSetting() ?? new Board();
            CurrentEngine.SendMessage("position fen " + board.GenerateFEN());
            CurrentEngine.SendMessage("go perft " + PerftDepth.Value);
        }

        private Board GetBoardForSetting()
        {
            switch ((PerftBoardSetting.SelectedItem as ComboBoxItem).Name)
            {
                case "CurrentBoard": return Game.Board;
                case "RegularBoard": return new Board();
                case "Kiwipete": return new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
                case "Position3": return new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
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
            BoardControl.ResetBoard();
            Game.Clear();

            if (CurrentEngine != null)
            {
                AnalysisOutput.Text = "";
                CurrentEngine.OutputReceived -= CurrentEngine_OutputReceived;
                CurrentEngine.Dispose();
                CurrentEngine = null;
                PerftButton.IsEnabled = false;
            }

            if ((string)EngineComboBox.SelectedItem != SELECT_ENGINE_PLACEHOLDER_TEXT)
            {
                CurrentEngine = new EngineProcess((string)EngineComboBox.SelectedItem);
                CurrentEngine.OutputReceived += CurrentEngine_OutputReceived;
                CurrentEngine.SendMessage("isready");
                PerftButton.IsEnabled = true;
            }
        }

        private void CurrentEngine_OutputReceived(string line)
        {
            this.Dispatcher.Invoke(() =>
            {
                AnalysisOutput.Text += line + "\n";
                OutputScrollViewer.ScrollToEnd();
            });
        }
    }
}

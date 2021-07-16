using ExtraChess.Analysis;
using ExtraChess.Models;
using NUnit.Framework;

namespace ExtraChessTests.Analysis
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PerftStartPos()
        {
            Board board = new Board();
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(20, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(400, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(8_902, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(197_281, PerftAnalyzer.Perft(board, 4));
            Assert.AreEqual(4_865_609, PerftAnalyzer.Perft(board, 5));

            // Uncomment when reaching better performance
            //Assert.AreEqual(119_060_324, PerftAnalyzer.Perft(board, 6));
            //Assert.AreEqual(3_195_901_860, PerftAnalyzer.Perft(board, 7));
            //Assert.AreEqual(84_998_978_956, PerftAnalyzer.Perft(board, 8));
            //Assert.AreEqual(2_439_530_234_167, PerftAnalyzer.Perft(board, 9));
            //Assert.AreEqual(69_352_859_712_417, PerftAnalyzer.Perft(board, 10));
            //Assert.AreEqual(2_097_651_003_696_806, PerftAnalyzer.Perft(board, 11));
            //Assert.AreEqual(62_854_969_236_701_747, PerftAnalyzer.Perft(board, 12));
            //Assert.AreEqual(1_981_066_775_000_396_239, PerftAnalyzer.Perft(board, 13));
        }

        [Test]
        public void PerftKiwiPete()
        {
            Board board = new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(48, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(2_039, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(97_862, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(4_085_603, PerftAnalyzer.Perft(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(193_690_690, PerftAnalyzer.Perft(board, 5));
            //Assert.AreEqual(8_031_647_685, PerftAnalyzer.Perft(board, 6));
        }

        [Test]
        public void PerftPosition3()
        {
            Board board = new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(14, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(191, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(2_812, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(43_238, PerftAnalyzer.Perft(board, 4));
            Assert.AreEqual(674_624, PerftAnalyzer.Perft(board, 5));
            Assert.AreEqual(11_030_083, PerftAnalyzer.Perft(board, 6));

            // Uncomment when reaching better performance
            //Assert.AreEqual(178_633_661, PerftAnalyzer.Perft(board, 7));
            //Assert.AreEqual(3_009_794_393, PerftAnalyzer.Perft(board, 8));
        }

        [Test]
        public void PerftPosition4()
        {
            Board board = new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(6, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(264, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(9_467, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(422_333, PerftAnalyzer.Perft(board, 4));
            Assert.AreEqual(15_833_292, PerftAnalyzer.Perft(board, 5));

            // Uncomment when reaching better performance
            //Assert.AreEqual(706_045_033, PerftAnalyzer.Perft(board, 6));
        }

        [Test]
        public void PerftPosition5()
        {
            Board board = new Board("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(44, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(1_486, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(62_379, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(2_103_487, PerftAnalyzer.Perft(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(89_941_194, PerftAnalyzer.Perft(board, 5));
        }

        [Test]
        public void PerftPosition6()
        {
            Board board = new Board("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            Assert.AreEqual(1, PerftAnalyzer.Perft(board, 0));
            Assert.AreEqual(46, PerftAnalyzer.Perft(board, 1));
            Assert.AreEqual(2_079, PerftAnalyzer.Perft(board, 2));
            Assert.AreEqual(89_890, PerftAnalyzer.Perft(board, 3));
            Assert.AreEqual(3_894_594, PerftAnalyzer.Perft(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(164_075_551, PerftAnalyzer.Perft(board, 5));
            //Assert.AreEqual(6_923_051_137, PerftAnalyzer.Perft(board, 6));
            //Assert.AreEqual(287_188_994_746, PerftAnalyzer.Perft(board, 7));
            //Assert.AreEqual(11_923_589_843_526, PerftAnalyzer.Perft(board, 8));
            //Assert.AreEqual(490_154_852_788_714, PerftAnalyzer.Perft(board, 9));
        }
    }
}
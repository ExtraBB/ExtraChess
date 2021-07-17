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
        public void PerftKiwiPeteConcurrent()
        {
            Board board = new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            Assert.AreEqual(48, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(2_039, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(97_862, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(4_085_603, Perft.PerftConcurrent(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(193_690_690, Perft.PerftConcurrent(board, 5));
            //Assert.AreEqual(8_031_647_685, Perft.PerftConcurrent(board, 6));
        }

        [Test]
        public void PerftKiwiPeteSync()
        {
            Board board = new Board("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            Assert.AreEqual(48, Perft.PerftSync(board, 1));
            Assert.AreEqual(2_039, Perft.PerftSync(board, 2));
            Assert.AreEqual(97_862, Perft.PerftSync(board, 3));
            Assert.AreEqual(4_085_603, Perft.PerftSync(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(193_690_690, Perft.PerftSync(board, 5));
            //Assert.AreEqual(8_031_647_685, Perft.PerftSync(board, 6));
        }

        [Test]
        public void PerftPosition3Concurrent()
        {
            Board board = new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            Assert.AreEqual(14, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(191, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(2_812, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(43_238, Perft.PerftConcurrent(board, 4));
            Assert.AreEqual(674_624, Perft.PerftConcurrent(board, 5));
            Assert.AreEqual(11_030_083, Perft.PerftConcurrent(board, 6));

            // Uncomment when reaching better performance
            //Assert.AreEqual(178_633_661, Perft.PerftConcurrent(board, 7));
            //Assert.AreEqual(3_009_794_393, Perft.PerftConcurrent(board, 8));
        }

        [Test]
        public void PerftPosition3Sync()
        {
            Board board = new Board("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            Assert.AreEqual(14, Perft.PerftSync(board, 1));
            Assert.AreEqual(191, Perft.PerftSync(board, 2));
            Assert.AreEqual(2_812, Perft.PerftSync(board, 3));
            Assert.AreEqual(43_238, Perft.PerftSync(board, 4));
            Assert.AreEqual(674_624, Perft.PerftSync(board, 5));
            Assert.AreEqual(11_030_083, Perft.PerftSync(board, 6));

            // Uncomment when reaching better performance
            //Assert.AreEqual(178_633_661, Perft.PerftSync(board, 7));
            //Assert.AreEqual(3_009_794_393, Perft.PerftSync(board, 8));
        }

        [Test]
        public void PerftPosition4Concurrent()
        {
            Board board = new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
            Assert.AreEqual(6, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(264, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(9_467, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(422_333, Perft.PerftConcurrent(board, 4));
            Assert.AreEqual(15_833_292, Perft.PerftConcurrent(board, 5));

            // Uncomment when reaching better performance
            //Assert.AreEqual(706_045_033, Perft.PerftConcurrent(board, 6));
        }

        [Test]
        public void PerftPosition4Sync()
        {
            Board board = new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
            Assert.AreEqual(6, Perft.PerftSync(board, 1));
            Assert.AreEqual(264, Perft.PerftSync(board, 2));
            Assert.AreEqual(9_467, Perft.PerftSync(board, 3));
            Assert.AreEqual(422_333, Perft.PerftSync(board, 4));
            Assert.AreEqual(15_833_292, Perft.PerftSync(board, 5));

            // Uncomment when reaching better performance
            //Assert.AreEqual(706_045_033, Perft.PerftSync(board, 6));
        }

        [Test]
        public void PerftPosition5Concurrent()
        {
            Board board = new Board("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
            Assert.AreEqual(44, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(1_486, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(62_379, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(2_103_487, Perft.PerftConcurrent(board, 4));
            Assert.AreEqual(89_941_194, Perft.PerftConcurrent(board, 5));
        }

        [Test]
        public void PerftPosition5Sync()
        {
            Board board = new Board("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
            Assert.AreEqual(44, Perft.PerftSync(board, 1));
            Assert.AreEqual(1_486, Perft.PerftSync(board, 2));
            Assert.AreEqual(62_379, Perft.PerftSync(board, 3));
            Assert.AreEqual(2_103_487, Perft.PerftSync(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(89_941_194, Perft.PerftSync(board, 5));
        }

        [Test]
        public void PerftPosition6Concurrent()
        {
            Board board = new Board("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            Assert.AreEqual(46, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(2_079, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(89_890, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(3_894_594, Perft.PerftConcurrent(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(164_075_551, Perft.PerftConcurrent(board, 5));
            //Assert.AreEqual(6_923_051_137, Perft.PerftConcurrent(board, 6));
            //Assert.AreEqual(287_188_994_746, Perft.PerftConcurrent(board, 7));
            //Assert.AreEqual(11_923_589_843_526, Perft.PerftConcurrent(board, 8));
            //Assert.AreEqual(490_154_852_788_714, Perft.PerftConcurrent(board, 9));
        }

        [Test]
        public void PerftPosition6Sync()
        {
            Board board = new Board("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            Assert.AreEqual(46, Perft.PerftSync(board, 1));
            Assert.AreEqual(2_079, Perft.PerftSync(board, 2));
            Assert.AreEqual(89_890, Perft.PerftSync(board, 3));
            Assert.AreEqual(3_894_594, Perft.PerftSync(board, 4));

            // Uncomment when reaching better performance
            //Assert.AreEqual(164_075_551, Perft.PerftSync(board, 5));
            //Assert.AreEqual(6_923_051_137, Perft.PerftSync(board, 6));
            //Assert.AreEqual(287_188_994_746, Perft.PerftSync(board, 7));
            //Assert.AreEqual(11_923_589_843_526, Perft.PerftSync(board, 8));
            //Assert.AreEqual(490_154_852_788_714, Perft.PerftSync(board, 9));
        }

        [Test]
        public void PerftStartPosConcurrent()
        {
            Board board = new Board();
            Assert.AreEqual(20, Perft.PerftConcurrent(board, 1));
            Assert.AreEqual(400, Perft.PerftConcurrent(board, 2));
            Assert.AreEqual(8_902, Perft.PerftConcurrent(board, 3));
            Assert.AreEqual(197_281, Perft.PerftConcurrent(board, 4));
            Assert.AreEqual(4_865_609, Perft.PerftConcurrent(board, 5));
            Assert.AreEqual(119_060_324, Perft.PerftConcurrent(board, 6));

            // Uncomment when reaching better performance
            //Assert.AreEqual(3_195_901_860, Perft.PerftConcurrent(board, 7));
            //Assert.AreEqual(84_998_978_956, Perft.PerftConcurrent(board, 8));
            //Assert.AreEqual(2_439_530_234_167, Perft.PerftConcurrent(board, 9));
            //Assert.AreEqual(69_352_859_712_417, Perft.PerftConcurrent(board, 10));
            //Assert.AreEqual(2_097_651_003_696_806, Perft.PerftConcurrent(board, 11));
            //Assert.AreEqual(62_854_969_236_701_747, Perft.PerftConcurrent(board, 12));
            //Assert.AreEqual(1_981_066_775_000_396_239, Perft.PerftConcurrent(board, 13));
        }

        [Test]
        public void PerftStartPosSync()
        {
            Board board = new Board();
            Assert.AreEqual(20, Perft.PerftSync(board, 1));
            Assert.AreEqual(400, Perft.PerftSync(board, 2));
            Assert.AreEqual(8_902, Perft.PerftSync(board, 3));
            Assert.AreEqual(197_281, Perft.PerftSync(board, 4));
            Assert.AreEqual(4_865_609, Perft.PerftSync(board, 5));

            // Uncomment when reaching better performance
            //Assert.AreEqual(119_060_324, Perft.PerftSync(board, 6));
            //Assert.AreEqual(3_195_901_860, Perft.PerftSync(board, 7));
            //Assert.AreEqual(84_998_978_956, Perft.PerftSync(board, 8));
            //Assert.AreEqual(2_439_530_234_167, Perft.PerftSync(board, 9));
            //Assert.AreEqual(69_352_859_712_417, Perft.PerftSync(board, 10));
            //Assert.AreEqual(2_097_651_003_696_806, Perft.PerftSync(board, 11));
            //Assert.AreEqual(62_854_969_236_701_747, Perft.PerftSync(board, 12));
            //Assert.AreEqual(1_981_066_775_000_396_239, Perft.PerftSync(board, 13));
        }
    }
}
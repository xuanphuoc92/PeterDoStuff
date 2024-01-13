using FluentAssertions;
using static PeterDoStuff.GameOfLife;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class GameOfLife_Test
    {
        [TestMethod]
        public void _01_DeadDueToIsolation()
        {
            // [O]
            using var game = new GameOfLife(1, 1);
            game.GetCell(1, 1).State = CellState.Live;
            game.Next();
            game.GetCell(1, 1).State.Should().Be(CellState.Dead);
        }

        [TestMethod]
        public void _02_StayLive()
        {
            // [ ][O][ ]
            // [O][ ][O]
            // [ ][O][ ]
            using var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.Next();            

            for (int x = 1; x <= 3; x++)
                for (int y = 1; y <= 3; y++)
                {
                    if (x + y == 3 || x + y == 5)
                        game.GetCell(x, y).State.Should().Be(CellState.Live);
                    else
                        game.GetCell(x, y).State.Should().Be(CellState.Dead);
                }
        }

        [TestMethod]
        public void _03_CenterDeadDueToOverpopulation_CornerResurrect()
        {
            // From:
            // [ ][O][ ]
            // [O][O][O]
            // [ ][O][ ]

            // To:
            // [O][O][O]
            // [O][ ][O]
            // [O][O][O]

            using var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.GetCell(2, 2).State = CellState.Live;
            
            game.Next();

            for (int x = 1; x <= 3; x++)
                for (int y = 1; y <= 3; y++)
                {
                    if (x == 2 && y == 2)
                        game.GetCell(x, y).State.Should().Be(CellState.Dead);
                    else
                        game.GetCell(x, y).State.Should().Be(CellState.Live);
                }
        }
    }
}
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
            // [X]
            var game = new GameOfLife(1, 1);
            game.GetCell(1, 1).State = CellState.Live;
            game.Next();
            game.GetCell(1, 1).State.Should().Be(CellState.Dead);
        }

        [TestMethod]
        public void _02_StayLive()
        {
            // [ ][X][ ]
            // [X][ ][X]
            // [ ][X][ ]
            var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.Next();            
            game.GetCell(1, 2).State.Should().Be(CellState.Live);
            game.GetCell(2, 1).State.Should().Be(CellState.Live);
            game.GetCell(3, 2).State.Should().Be(CellState.Live);
            game.GetCell(2, 3).State.Should().Be(CellState.Live);
        }

        [TestMethod]
        public void _03_DeadDueToOverpopulation()
        {
            // [ ][X][ ]
            // [X][X][X]
            // [ ][X][ ]
            var game = new GameOfLife(3, 3);
            game.GetCell(1, 2).State = CellState.Live;
            game.GetCell(2, 1).State = CellState.Live;
            game.GetCell(3, 2).State = CellState.Live;
            game.GetCell(2, 3).State = CellState.Live;
            game.GetCell(2, 2).State = CellState.Live;
            game.Next();
            game.GetCell(2, 2).State.Should().Be(CellState.Dead);
        }
    }
}
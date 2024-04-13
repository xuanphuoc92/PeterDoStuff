using FluentAssertions;
using PeterDoStuff.Games._2048;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test
{
    [TestClass]
    public class _2048_Test
    {
        [TestMethod]
        public void _01_Start()
        {
            var game = new Game(0, 2);
            game.Blocks.Should().HaveCount(2);
            
            game.Blocks[0].Number.Should().Be(2);
            game.Blocks[0].LocationIndex.Should().Be(0);
            game.Blocks[0].X.Should().Be(0);
            game.Blocks[0].Y.Should().Be(0);

            game.Blocks[1].Number.Should().Be(2);
            game.Blocks[1].LocationIndex.Should().Be(2);
            game.Blocks[1].X.Should().Be(2);
            game.Blocks[1].Y.Should().Be(0);

            game = new Game();
            game.Blocks.Should().HaveCount(game.StartBlocks);            
        }

        private static void ContainBlock(Game game, int locationIndex, int number)
        {
            game.Blocks.Any(b => b.LocationIndex == locationIndex && b.Number == number).Should().BeTrue();
        }

        [TestMethod]
        public void _02_Down()
        {
            // [2][2][ ][ ]
            // [2][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(0, 1, 4);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Down();

            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [4][2][ ][ ]
            ContainBlock(game, 12, 4);
            ContainBlock(game, 13, 2);            

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _03_Up()
        {
            // [2][2][ ][ ]
            // [2][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(0, 1, 4);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Up();

            // [4][2][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]

            ContainBlock(game, 0, 4);
            ContainBlock(game, 1, 2);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _04_NothingChange()
        {
            // [2][2][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(0, 1);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(2);

            game.Up(); // Nothing should change

            ContainBlock(game, 0, 2);
            ContainBlock(game, 1, 2);

            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(2);
        }

        [TestMethod]
        public void _05_Right()
        {
            // [2][2][ ][ ]
            // [2][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(0, 1, 4);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Right();

            // [ ][ ][ ][4]
            // [ ][ ][ ][2]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]

            ContainBlock(game, 3, 4);
            ContainBlock(game, 7, 2);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _06_Left()
        {
            // [2][2][ ][ ]
            // [2][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(0, 1, 4);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Left();

            // [4][ ][ ][ ]
            // [2][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]

            ContainBlock(game, 0, 4);
            ContainBlock(game, 4, 2);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _07_GameOver()
        {
            var game = Game.GameOverTest();
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(16);
            game.IsGameOver.Should().BeFalse();

            game.Left();

            game.IsGameOver.Should().BeTrue();
        }

        [TestMethod]
        public void _08_ChainMerging()
        {
            // [4][2][2][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game((0, 4), (1, 2), (2, 2));
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Right();

            // [ ][ ][4][4]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]

            ContainBlock(game, 2, 4);
            ContainBlock(game, 3, 4);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _09_Grid()
        {
            // [4][2][2][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game((0, 4), (1, 2), (2, 2));
            Grid grid = game.Grid;

            grid.Cells.Should().HaveCount(16);
            grid.Cells[(0, 0)].State.Should().Be(CellState.Appear);
            grid.Cells[(1, 0)].State.Should().Be(CellState.Appear);
            grid.Cells[(2, 0)].State.Should().Be(CellState.Appear);
            grid.Cells[(3, 0)].State.Should().Be(CellState.Empty);

            game.Up();

            grid.Cells[(0, 0)].State.Should().Be(CellState.Stay);
            grid.Cells[(1, 0)].State.Should().Be(CellState.Stay);
            grid.Cells[(2, 0)].State.Should().Be(CellState.Stay);
            grid.Cells[(3, 0)].State.Should().Be(CellState.Empty);

            game.Right();

            // [ ][ ][4][4]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]

            grid.Cells[(2, 0)].State.Should().Be(CellState.MoveIn);
            grid.Cells[(2, 0)].MoveInX.Should().Be(2);
            grid.Cells[(2, 0)].MoveInY.Should().Be(0);

            grid.Cells[(3, 0)].State.Should().Be(CellState.MoveIn);
            grid.Cells[(3, 0)].MoveInX.Should().Be(2);
            grid.Cells[(3, 0)].MoveInY.Should().Be(0);

            grid.Cells.Values.Where(c => c.State == CellState.Appear).Should().HaveCount(1);
            grid.Cells.Values.Where(c => c.State == CellState.MoveIn).Should().HaveCount(2);

            game.Up();

            grid.Cells[(2, 0)].State.Should().Be(CellState.Stay);
            grid.Cells[(3, 0)].State.Should().Be(CellState.Stay);

            grid.Cells.Values.Where(c => c.State == CellState.Stay).Count().Should().BeGreaterThanOrEqualTo(2);
        }

        [TestMethod]
        public void _10_VariantGame()
        {
            var game = new VariantGame(6, 6, 3, 2);
            game.Width.Should().Be(6);
            game.Height.Should().Be(6);
            game.Blocks.Should().HaveCount(3);
        }
    }
}
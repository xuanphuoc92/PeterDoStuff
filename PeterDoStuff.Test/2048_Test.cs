using FluentAssertions;
using PeterDoStuff._2048;
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
        public void _01_StartBlocks()
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
            game.Blocks.Should().HaveCount(Game.START_BLOCKS);            
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
    }
}
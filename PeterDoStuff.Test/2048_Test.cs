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
            game.Blocks[0].LocationIndex.Should().Be(12);
            game.Blocks[0].Number.Should().Be(4);

            game.Blocks[1].LocationIndex.Should().Be(13);
            game.Blocks[1].Number.Should().Be(2);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        [TestMethod]
        public void _03_Up()
        {
            // [ ][2][ ][ ]
            // [2][2][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            var game = new Game(1, 4, 5);
            game.Score.Should().Be(0);
            game.Blocks.Should().HaveCount(3);

            game.Up();

            // [2][4][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            // [ ][ ][ ][ ]
            game.Blocks[0].LocationIndex.Should().Be(0);
            game.Blocks[0].Number.Should().Be(2);

            game.Blocks[1].LocationIndex.Should().Be(1);
            game.Blocks[1].Number.Should().Be(4);

            game.Score.Should().Be(4);
            game.Blocks.Should().HaveCount(3);
        }

        //[TestMethod]
        //public void _03_NothinChange()
        //{
        //    // [2][2][ ][ ]
        //    // [2][ ][ ][ ]
        //    // [ ][ ][ ][ ]
        //    // [ ][ ][ ][ ]
        //    var game = new Game(0, 1, 4);
        //    game.Score.Should().Be(0);
        //    game.Blocks.Should().HaveCount(3);

        //    game.Up(); // Nothing should change

        //    game.Blocks[0].LocationIndex.Should().Be(0);
        //    game.Blocks[1].LocationIndex.Should().Be(0);
        //    game.Blocks[2].LocationIndex.Should().Be(0);
        //    game.Score.Should().Be(0);
        //    game.Blocks.Should().HaveCount(3);
        //}
    }
}

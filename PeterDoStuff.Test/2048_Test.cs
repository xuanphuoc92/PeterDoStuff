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
    }
}

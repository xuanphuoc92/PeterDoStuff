using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff._2048
{
    public class Game
    {
        public List<Block> Blocks { get; private set; }
        
        public int Width { get; private set; } = 4;
        public int Height { get; private set; } = 4;
        
        public const int START_BLOCKS = 2;

        public Game()
        {
            var allBlocks = Enumerable.Range(0, Width * Height);
            var random = new Random();
            allBlocks.OrderBy(b => random.Next());
            var startBlockLocations = allBlocks
                .OrderBy(b => random.Next()) // Shuffle
                .Take(START_BLOCKS); // Take the first 2

            SetupStartBlocks(startBlockLocations);
        }

        public Game(params int[] startBlockLocations)
        {
            SetupStartBlocks(startBlockLocations);
        }

        private void SetupStartBlocks(IEnumerable<int> startBlockLocations)
        {
            Blocks = new List<Block>();
            foreach (var location in startBlockLocations)
                Blocks.Add(new Block(location, this));
        }
    }

    public class Block
    {
        public int Number { get; private set; }
        public int LocationIndex { get; private set; }
        public Game Game { get; private set; }

        public Block(int locationIndex, Game game)
        {
            Number = 2; // Every block created must be 2
            LocationIndex = locationIndex;
            Game = game;
        }

        public int X
        {
            get => LocationIndex % Game.Width;
        }

        public int Y
        {
            get => LocationIndex / Game.Height;
        }
    }
}

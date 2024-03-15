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

        public int Score { get; internal set; } = 0;

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

        public Game Down()
        {
            Blocks
                .OrderByDescending(b => b.Y) // Ordered blocks from bottom to top
                .ToList()
                .ForEach(b => b.Down()); // Tell the blocks to move down
            return this;
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

        internal Block Down()
        {
            Block? destBlock = Game.Blocks
                .Where(b => b.X == this.X && b.Y > this.Y) // Same Column, any other block
                .MaxBy(b => b.Y); // First that it can hit
            int destX = X;
            int destY = destBlock != null ? (destBlock.Y - 1) : (Game.Height - 1);
            MoveAndMerge(destBlock, destX, destY);
            return this;
        }

        private void MoveAndMerge(Block? destBlock, int destX, int destY)
        {
            if (destBlock != null && destBlock.Number == Number) // Merge
            {
                this.LocationIndex = destBlock.LocationIndex;
                Game.Blocks.Remove(destBlock);
                Number *= 2;
                Game.Score += Number;
            }
            else // Not Merge
            {
                this.LocationIndex = Game.Width * destY + destX;
            }
        }
    }
}

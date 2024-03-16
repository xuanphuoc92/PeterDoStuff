using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff._2048
{
    public class Game
    {
        public IEnumerable<Block> Blocks => AllBlocks.Where(b => b.Deleted == false);
        public List<Block> AllBlocks { get; private set; } = new List<Block>();

        public int Width { get; private set; } = 4;
        public int Height { get; private set; } = 4;

        public int Size => Width * Height;
        
        public const int START_BLOCKS = 2;

        public int Score { get; internal set; } = 0;

        public bool IsGameOver { get; private set; } = false;

        public Game()
        {
            var allLocs = Enumerable.Range(0, Size);
            var random = new Random();
            allLocs.OrderBy(loc => random.Next());
            var startBlockLocations = allLocs
                .OrderBy(loc => random.Next()) // Shuffle
                .Take(START_BLOCKS); // Take the first 2

            SetupStartBlocks(startBlockLocations);
        }

        public Game(params int[] startBlockLocations)
        {
            SetupStartBlocks(startBlockLocations);
        }

        public static Game GameOverTest()
        {
            var game = new Game();
            game.AllBlocks.Clear();
            for (int i = 0; i < game.Size; i++)
                game.AllBlocks.Add(new Block(i, game, i));
            return game;
        }

        private void SetupStartBlocks(IEnumerable<int> startBlockLocations)
        {            
            foreach (var location in startBlockLocations)
                AllBlocks.Add(new Block(location, this));
        }

        public Game(params (int Loc, int Number)[] blocks)
        {
            foreach (var block in blocks)
                AllBlocks.Add(new Block(block.Loc, this, block.Number));
        }

        internal bool AnyMovement { get; set; } = false;

        private void PreMovement()
        {
            AnyMovement = false;
        }

        private void PostMovement()
        {
            if (AnyMovement == false)
            {
                if (Blocks.Count() == Size)
                    IsGameOver = true;
                return;
            }

            var occupied = Blocks.Select(b => b.LocationIndex);

            var random = new Random();
            var randomEmpty = Enumerable.Range(0, Size)
                .Where(loc => occupied.Contains(loc) == false)
                .OrderBy(loc => random.Next())
                .First();

            AllBlocks.Add(new Block(randomEmpty, this));
            
            AnyMovement = false;

            Blocks.Where(b => b.Merged == true).ToList().ForEach(b => b.Merged = false);
        }

        public Game Down()
        {
            PreMovement();
            Blocks
                .OrderByDescending(b => b.Y) // Ordered blocks from bottom to top
                .ToList()
                .ForEach(b => b.Down()); // Tell the blocks to move down
            PostMovement();
            return this;
        }


        public Game Up()
        {
            PreMovement();
            Blocks
                .OrderBy(b => b.Y) // Ordered blocks from top to bottom
                .ToList()
                .ForEach(b => b.Up()); // Tell the blocks to move down
            PostMovement();
            return this;
        }

        public Game Right()
        {
            PreMovement();
            Blocks
                .OrderByDescending(b => b.X) // Ordered blocks from right to left
                .ToList()
                .ForEach(b => b.Right()); // Tell the blocks to move right
            PostMovement();
            return this;
        }

        public Game Left()
        {
            PreMovement();
            Blocks
                .OrderBy(b => b.X) // Ordered blocks from left to right
                .ToList()
                .ForEach(b => b.Left()); // Tell the blocks to move left
            PostMovement();
            return this;
        }
    }

    public class Block
    {
        public int Number { get; private set; }
        public int StartLocationIndex { get; private set; }
        public int LocationIndex { get; private set; }
        public Game Game { get; private set; }
        public bool Deleted { get; private set; }
        public bool Merged { get; set; }

        public Block(int locationIndex, Game game, int number = 2)
        {
            Number = number;
            StartLocationIndex = locationIndex;
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

        public (int X, int Y) StartLocation => (StartLocationIndex % Game.Width, StartLocationIndex / Game.Height);

        internal void Down()
        {
            Block? destBlock = Game.Blocks
                .Where(b => b.X == this.X && b.Y > this.Y) // Same Column, any other block below it
                .MinBy(b => b.Y); // First that it can hit
            int destX = X;
            int destY = destBlock != null ? (destBlock.Y - 1) : (Game.Height - 1);
            MoveAndMerge(destBlock, destX, destY);
        }

        internal void Up()
        {
            Block? destBlock = Game.Blocks
                .Where(b => b.X == this.X && b.Y < this.Y) // Same Column, any other block above it
                .MaxBy(b => b.Y); // First that it can hit
            int destX = X;
            int destY = destBlock != null ? (destBlock.Y + 1) : 0;
            MoveAndMerge(destBlock, destX, destY);
        }

        internal void Right()
        {
            Block? destBlock = Game.Blocks
                .Where(b => b.Y == this.Y && b.X > this.X) // Same Row, any other block on its right
                .MinBy(b => b.X); // First that it can hit
            int destX = destBlock != null ? (destBlock.X - 1) : (Game.Width - 1);
            int destY = Y;
            MoveAndMerge(destBlock, destX, destY);
        }

        internal void Left()
        {
            Block? destBlock = Game.Blocks
                .Where(b => b.Y == this.Y && b.X < this.X) // Same Row, any other block on its left
                .MaxBy(b => b.X); // First that it can hit
            int destX = destBlock != null ? (destBlock.X + 1) : 0;
            int destY = Y;
            MoveAndMerge(destBlock, destX, destY);
        }

        private void MoveAndMerge(Block? destBlock, int destX, int destY)
        {
            if (destBlock != null && destBlock.Number == Number && destBlock.Merged == false) // Merge
            {
                this.LocationIndex = destBlock.LocationIndex;
                //Game.Blocks.Remove(destBlock);
                destBlock.Deleted = true;
                Number *= 2;
                Game.Score += Number;
                Game.AnyMovement = true;
                Merged = true;
            }
            else // Not Merge
            {
                int newLocation = Game.Width * destY + destX;
                if (LocationIndex != newLocation)
                {
                    LocationIndex = newLocation;
                    Game.AnyMovement = true;
                }
            }
        }
    }
}

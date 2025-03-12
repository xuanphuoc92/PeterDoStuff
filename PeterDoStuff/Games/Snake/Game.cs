using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Games.Snake
{
    public class Game
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<(int X, int Y), Cell> Cells { get; private set; } = new Dictionary<(int X, int Y), Cell>();

        private Snake? _snake = null;
        public Snake Snake => _snake ??= new Snake(this);

        public Cell Bait { get; private set; }

        public GameState State { get; internal set; } = GameState.Playing;

        public Game(int width, int height, int snakeStartLength = 2)
        {
            if (width / 2 + 1 < snakeStartLength)
                throw new Exception($"Game with width of {width} is not suitable for snake with start length {snakeStartLength}.");

            Width = width;
            Height = height;

            // Create the game cells
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[(x, y)] = new Cell(x, y);
                }
            }

            // Construct the snake
            var startX = Width / 2;
            var startY = Height / 2;

            for (int x = startX - snakeStartLength + 1; x <= startX; x++)
            {
                Snake.Add(x, startY);
            }

            // Add the Bait
            AddRandomBait();
        }

        internal void AddRandomBait()
        {
            var rnd = new Random();
            var bait = Cells.Values
                .Where(c => c.State == CellState.Empty)
                .OrderBy(c => rnd.Next())
                .FirstOrDefault();

            if (bait == null)
            {
                State = GameState.Win;
            }
            else
            {
                SetBait(bait);
            }
        }

        private void SetBait(Cell bait)
        {
            Bait = bait;
            Bait.State = CellState.Bait;
        }

        public void SwitchBait(int x, int y)
        {
            Bait.State = CellState.Empty;
            SetBait(Cells[(x, y)]);
        }

        public void Step()
        {
            if (State != GameState.Playing) return;
            Snake.Step();
        }

        public void Up()
        {
            if (Snake.Direction != Direction.Down)
            {
                Snake.Direction = Direction.Up;
                Step();
            }
        }

        public void Down()
        {
            if (Snake.Direction != Direction.Up)
            {
                Snake.Direction = Direction.Down;
                Step();
            }
        }

        public void Left()
        {
            if (Snake.Direction != Direction.Right)
            {
                Snake.Direction = Direction.Left;
                Step();
            }
        }

        public void Right()
        {
            if (Snake.Direction != Direction.Left)
            {
                Snake.Direction = Direction.Right;
                Step();
            }
        }
    }

    public enum GameState
    {
        Playing, Win, Over
    }

    public class Snake
    {
        public Game Game { get; private set; }
        public Queue<Cell> Cells { get; private set; } = new Queue<Cell>();
        public Direction Direction { get; internal set; } = Direction.Right;
        public Cell Head => Cells.Last();
        public Cell Tail => Cells.First();

        public Snake(Game game)
        {
            Game = game;
        }

        public void Add(int x, int y)
        {
            Cell cell = Game.Cells[(x, y)];
            Add(cell);
        }

        private void Add(Cell cell)
        {
            cell.State = CellState.Snake;
            Cells.Enqueue(cell);
        }

        public int Length => Cells.Count;

        internal void Step()
        {
            var head = Head;

            var nextX = Direction switch
            {
                Direction.Right => head.X + 1,
                Direction.Left => head.X - 1,
                _ => head.X
            };

            var nextY = Direction switch
            {
                Direction.Down => head.Y + 1,
                Direction.Up => head.Y - 1,
                _ => head.Y
            };

            // Eat Wall
            if (Game.Cells.ContainsKey((nextX, nextY)) == false)
            {
                Game.State = GameState.Over;
                return;
            }

            Cell nextCell = Game.Cells[(nextX, nextY)];

            if (nextCell.State == CellState.Snake && nextCell != Tail)
            {
                Game.State = GameState.Over;
                return;
            }
            else if (nextCell.State == CellState.Bait)
            {
                Add(nextCell);
                Game.AddRandomBait();
            }
            else
            {
                Add(nextCell);
                PopTail();
            }
        }

        private void PopTail()
        {
            var tail = Cells.Dequeue();
            tail.State = CellState.Empty;
        }
    }

    public enum Direction { Left, Right, Up, Down }

    public class Cell
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public CellState State { get; internal set; } = CellState.Empty;
    }

    public enum CellState
    {
        Empty, Snake, Bait
    }
}

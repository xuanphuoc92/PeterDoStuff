using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PeterDoStuff.GameOfLife;

namespace PeterDoStuff
{
    public class MineSweeper : IDisposable
    {
        public void Dispose()
        {
            Cells.Clear();
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public GameState State { get; private set; }

        public Dictionary<(int, int), Cell> Cells { get; private set; }
            = new Dictionary<(int, int), Cell>();

        public int MineCount { get; private set; } = 0;

        public static MineSweeper New(int width, int height)
        {
            return new MineSweeper()
            {
                Width = width,
                Height = height
            }.Reset();
        }

        public MineSweeper Reset()
        {
            StartTime = EndTime = null;
            State = GameState.Playing;
            Cells.Clear();
            MineCount = 0;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Cells[(x, y)] = new Cell(x, y, this);

            return this;
        }

        public MineSweeper SetMine(int x, int y)
        {
            Cells[(x, y)].SetMine();
            MineCount++;
            return this;
        }

        public MineSweeper RandomizeMines(int mines)
        {
            Random rnd = new Random();
            Cells
                .Values // Take all Cells
                .OrderBy(c => rnd.Next()) // Order randomly
                .Take(mines) // Take first {mines} Cells
                .ToList().ForEach(c => c.SetMine()); // Set as Mines
            MineCount += mines;
            return this;
        }

        public MineSweeper Start()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    CalculateNumber(x, y);

            return this;
        }

        private void CalculateNumber(int x, int y)
        {
            Cell cell = Cells[(x, y)];
            if (cell.IsMine) return;
            List<Cell> aroundCells = GetAroundCells(x, y);
            cell.SetNumber(aroundCells.Count(c => c.IsMine));
        }

        private List<Cell> GetAroundCells(int x, int y)
        {
            List<Cell> results = new List<Cell>();
            for (int x2 = x - 1; x2 <= x + 1; x2++)
                for (int y2 = y - 1; y2 <= y + 1; y2++)
                {
                    if (x2 == x && y2 == y) continue;
                    if (Cells.ContainsKey((x2, y2)) == false) continue;
                    results.Add(Cells[(x2, y2)]);
                }
            return results;
        }

        public MineSweeper Pick(int x, int y)
        {
            if (StartTime == null) StartTime = DateTime.Now;

            if (State != GameState.Playing)
                return this;

            Cell cell = Cells[(x, y)];
            SpreadPick(cell);
            if (cell.IsMine == true)
                Lose();
            CheckWin();
            return this;
        }

        private void SpreadPick(Cell cell)
        {
            if (cell.IsPicked == true)
                return;
            cell.Pick();

            if (cell.IsMine == true || cell.Number > 0)
                return;

            List<Cell> aroundCells = GetAroundCells(cell.X, cell.Y);
            aroundCells.ForEach(c => SpreadPick(c));
        }

        private void CheckWin()
        {
            if (Cells.Values
                .Where(c => c.IsMine == false)
                .All(c => c.IsPicked == true))
            {
                State = GameState.Win;
                EndTime = DateTime.Now;
            }
        }

        private void Lose()
        {
            State = GameState.Lose;
            EndTime = DateTime.Now;
        }

        public MineSweeper Flag(int x, int y)
        {
            if (State != GameState.Playing)
                return this;

            Cell cell = Cells[(x, y)];
            cell.Flag();
            if (cell.State == CellState.Flagged) MineCount--;
            if (cell.State == CellState.Default) MineCount++;
            return this;
        }

        public int GetLiveSeconds()
        {
            if (StartTime == null) return 0;
            DateTime end = EndTime != null ? EndTime.Value : DateTime.Now;
            return (int)(end - StartTime.Value).TotalSeconds;
        }

        public enum GameState
        {
            Playing, Win, Lose
        }

        public class Cell
        {
            public CellState State { get; private set; }
            public bool IsMine { get; private set; }
            public bool IsPicked => State == CellState.Picked;
            public int Number { get; private set; }

            public int X { get; private set; }
            public int Y { get; private set; }
            private MineSweeper Game { get; set; }

            internal Cell(int x, int y, MineSweeper game)
            {
                X = x; Y = y; Game = game;
            }

            internal void Pick()
            {
                State = CellState.Picked;
            }

            internal void SetMine()
            {
                IsMine = true;
            }

            internal void SetNumber(int number)
            {
                Number = number;
            }

            internal void Flag()
            {
                // Picked Cell cannot be flagged
                if (State == CellState.Picked) return;

                if (State == CellState.Default)
                {
                    State = CellState.Flagged;
                    return;
                }
                if (State == CellState.Flagged)
                {
                    State = CellState.Default;
                    return;
                }
            }

            public CellContent Content
            {
                get
                {
                    if (IsPicked && Number > 0)
                        return CellContent.Number;
                    if (IsMine &&
                        (IsPicked || Game.State == GameState.Lose))
                        return CellContent.Mine;
                    if (State == CellState.Flagged)
                        return CellContent.Flag;
                    return CellContent.Empty;
                }
            }

            public CellBackground Background
            {
                get
                {
                    if (IsPicked && IsMine)
                        return CellBackground.PickedMine;
                    if (State == CellState.Flagged && IsMine == false && Game.State == GameState.Lose)
                        return CellBackground.WrongFlag;
                    if (IsPicked)
                        return CellBackground.Picked;
                    return CellBackground.Default;
                }
            }
        }

        public enum CellState
        {
            Default, Picked, Flagged
        }

        public enum CellContent
        {
            Empty, Number, Mine, Flag
        }

        public enum CellBackground
        {
            Default, Picked, WrongFlag, PickedMine
        }
    }
}

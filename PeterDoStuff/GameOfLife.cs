using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace PeterDoStuff
{
    public class GameOfLife
    {
        public GameOfLife(int width, int height)
        {
            Width = width;
            Height = height;
            InitCells();
        }

        private Dictionary<(int, int), Cell> _cells = new Dictionary<(int, int), Cell> ();
        private void InitCells()
        {
            // Create cells into the dictionary:
            for (int x = 1; x <= Width; x++)
                for (int y = 1; y <= Height; y++)
                {
                    var cell = new Cell(x, y);
                    _cells[(x, y)] = cell;
                }

            //  Register neighbours for each cells:
            for (int x = 1; x <= Width; x++)
                for (int y = 1; y <= Height; y++)
                {
                    var cell = _cells[(x, y)];
                    cell.RegisterNeighbours(this);
                }
        }

        public Cell GetCell(int x, int y) => _cells[(x, y)];

        public void Next()
        {
            foreach (var cell in _cells.Values)
            {
                cell.CountLiveNeighbours();
                cell.CalculateNext();
            }

            foreach (var cell in _cells.Values)
            {
                cell.UpdateToNext();
            }
        }

        public class Cell
        {
            public CellState State { get; set; } = CellState.Dead;
            public int X { get; private set; }
            public int Y { get; private set; }

            public Cell(int x, int y)
            {
                X = x;
                Y = y;
            }

            private List<Cell> _neighbours = new List<Cell>();

            internal void RegisterNeighbours(GameOfLife game)
            {
                for (int x = X-1; x <= X+1; x++)
                    for (int y = Y-1; y <= Y+1; y++)
                    {
                        if (x == 0 || y == 0) continue;
                        if (x == game.Width + 1 || y == game.Height + 1) continue;
                        if (x == X && y == Y) continue;

                        var cell = game._cells[(x, y)];
                        _neighbours.Add(cell);
                    }
            }

            private int _liveNeighbours = 0;
            internal void CountLiveNeighbours()
            {
                _liveNeighbours = _neighbours.Count(c => c.State == CellState.Live);
            }

            private CellState _nextState;
            internal void CalculateNext()
            {
                _nextState = State;
                if (State == CellState.Live)
                {
                    if (_liveNeighbours < 2 || _liveNeighbours > 3) 
                        _nextState = CellState.Dead;
                }
                else
                {
                    if (_liveNeighbours == 3)
                        _nextState = CellState.Live;
                }
            }

            internal void UpdateToNext()
            {
                State = _nextState;
            }
        }

        public enum CellState
        {
            Dead,
            Live
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}

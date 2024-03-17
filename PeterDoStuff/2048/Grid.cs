using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff._2048
{
    public class Grid
    {
        public Dictionary<(int X, int Y), Cell> Cells { get; internal set; } = new Dictionary<(int X, int Y), Cell>();

        internal Grid(Game game)
        {
            for (int x= 0; x< game.Width; x++) 
            {
                for (int y = 0; y< game.Height; y++)
                {
                    Cells.Add((x, y), new Cell());
                }
            }
        }

        internal void BlockAppear(Block block)
        {
            Cell cell = Cells[(block.X, block.Y)];
            cell.Block = block;
            cell.State = CellState.Appear;
        }
    }

    public class Cell
    {
        public Block? Block { get; internal set; }
        public CellState State { get; internal set; } = CellState.Empty;
    }

    public enum CellState
    {
        Empty, Appear, Stay, MoveIn
    }
}

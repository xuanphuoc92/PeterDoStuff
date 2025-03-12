namespace PeterDoStuff.Games._2048
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

        internal void BlockAppear(int x, int y, int number)
        {
            Cell cell = Cells[(x, y)];
            cell.Number = number;
            cell.State = CellState.Appear;
        }

        internal void TurnAllAppearAndMoveInToStay()
        {
            Cells.Values
                .Where(c => c.State == CellState.Appear || c.State == CellState.MoveIn)
                .ToList().ForEach(c => c.State = CellState.Stay);
        }

        internal void BlockMove(int prevX, int prevY, int x, int y, int number)
        {
            Cell prevCell = Cells[(prevX, prevY)];
            prevCell.State = CellState.Empty;
            
            Cell cell = Cells[(x, y)];
            cell.Number = number;
            cell.State = CellState.MoveIn;
            cell.MoveInX = x - prevX;
            cell.MoveInY = y - prevY;
        }
    }

    public class Cell
    {
        public int? Number { get; internal set; }
        public CellState State { get; internal set; } = CellState.Empty;
        public int MoveInX { get; internal set; }
        public int MoveInY { get; internal set; }
    }

    public enum CellState
    {
        Empty, Appear, Stay, MoveIn
    }
}

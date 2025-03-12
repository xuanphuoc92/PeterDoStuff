namespace PeterDoStuff.Games
{
    public class MatchFinder
    {
        public class Cell
        {
            public int Content { get; internal set; }
            public bool IsPicked { get; internal set; }
            public bool IsMatched { get; internal set; }
        }

        public List<Cell> Cells { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        private MatchFinder()
        {
            Reset();
        }

        public MatchFinder Reset()
        {
            Cells = new List<Cell>();
            StartTime = null;
            EndTime = null;
            _picked = new List<Cell>();
            return this;
        }

        public static MatchFinder New() { return new MatchFinder(); }
        public MatchFinder Add(int cellContent)
        {
            Cells.Add(new Cell() { Content = cellContent });
            return this;
        }

        private List<Cell> _picked = new List<Cell>();

        public MatchFinder Pick(int cellIndex)
        {
            Cell cell = Cells[cellIndex];
            return Pick(cell);
        }

        public MatchFinder Pick(Cell cell)
        {
            if (StartTime == null)
                StartTime = DateTime.Now;

            if (cell.IsPicked && _picked.Count < 2)
                return this;

            if (cell.IsMatched)
                return this;

            if (_picked.Count == 2)
            {
                Unpick();
            }

            cell.IsPicked = true;
            _picked.Add(cell);

            if (_picked.Count == 2)
            {
                Match();
            }

            return this;
        }

        private void Match()
        {
            if (_picked[0].Content == _picked[1].Content)
            {
                _picked[0].IsMatched = _picked[1].IsMatched = true;
                Unpick();
                CheckEnd();
            }
        }

        private void CheckEnd()
        {
            if (Cells.Any(c => c.IsMatched == false))
                return;

            EndTime = DateTime.Now;
        }

        private void Unpick()
        {
            _picked.ForEach(c => c.IsPicked = false);
            _picked.Clear();
        }

        public MatchFinder AddRandoms(int cells)
        {
            for (int i = 0; i < cells; i++)
                Add(-1);

            Random rnd = new Random();
            var randomlyOrdered = Cells.OrderBy(c => rnd.Next()).ToList();
            for (int i = 0; i < cells; i++)
                randomlyOrdered[i].Content = i / 2;

            return this;
        }
    }
}

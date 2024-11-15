namespace PeterDoStuff.Tools.Graphics
{
    public class PathModel : CompositeModel
    {
        public List<PathCommand> Commands { get; } = [];

        public PathModel(double x, double y)
        {
            MoveTo(x, y);
        }

        public PathModel MoveTo(double x, double y)
        {
            Commands.Add(new("M", AddPoint(x, y)));
            return this;
        }

        public PathModel LineTo(double x, double y)
        {
            Commands.Add(new("L", AddPoint(x, y)));
            return this;
        }

        public PathModel QuadCurveTo(double xs, double ys, double x, double y)
        {
            Model controlPoint = AddPoint(xs, ys);
            Commands.Add(new("Q", controlPoint, AddPoint(x, y)));
            return this;
        }

        public PathModel QuadCurveTo(double x, double y)
        {
            Commands.Add(new("T", AddPoint(x, y)));
            return this;
        }

        public PathModel ClosePath()
        {
            Commands.Add(new ("Z"));
            return this;
        }

        private Model AddPoint(double x, double y)
        {
            var point = new Model() { X = x, Y = y, Z = this.Z };
            Models.Add(point);
            return point;
        }
    }

    public class PathCommand(string command, params Model[] points)
    {
        public string Command = command;
        public Model[] Points = points;
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class PathModel : CompositeModel
    {
        public List<PathCommand> Commands { get; } = [];

        public PathModel(double x, double y) : base()
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

        public PathModel SmoothCurveTo(double x, double y, double xs, double ys)
        {
            Commands.Add(new("S", AddPoint(xs, ys), AddPoint(x, y)));
            return this;
        }

        private Model AddPoint(double x, double y)
        {
            var point = new Model() { X = x, Y = y, Z = this.Z };
            Children.Add(point);
            return point;
        }
    }

    public class PathCommand(string command, params Model[] points)
    {
        public string Command = command;
        public Model[] Points = points;
    }
}

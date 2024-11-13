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
            AddPathPoint(x, y);
            Commands.Add(new("M"));
            return this;
        }

        public PathModel LineTo(double x, double y)
        {
            AddPathPoint(x, y);
            Commands.Add(new("L"));
            return this;
        }

        private void AddPathPoint(double x, double y)
        {
            var model = new Model() { X = x, Y = y, Z = this.Z };
            Children.Add(model);
        }
    }

    public class PathCommand(string command, params double[] parameters)
    {
        public string Command = command;
        public double[] Params = parameters;
    }
}

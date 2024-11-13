namespace PeterDoStuff.Tools.Graphics
{
    public class Line : CompositeModel
    {
        public Model Start { get; } = new();
        public Model End { get; } = new();

        public Line() : base()
        {
            Children = [Start, End];
        }

        public Line Set(double startX, double startY, double endX, double endY)
        {
            Start.X = startX;
            Start.Y = startY;
            End.X = endX;
            End.Y = endY;
            return this;
        }
    }
}

using System.Net;

namespace PeterDoStuff.Tools.Graphics
{
    public class Polygon : CompositeModel
    {
        public List<Model> Points = [];

        public Polygon(double x1, double y1, double x2, double y2, double x3, double y3, params double[] points)
        {
            AddPoint(x1, y1);
            AddPoint(x2, y2);
            AddPoint(x3, y3);
            for (int i = 0; i < points.Length; i += 2)
            {
                var x = points[i];
                var y = points[i + 1];
                AddPoint(x, y);
            }
        }

        private void AddPoint(double x, double y)
        {
            var point = new Model() { X = x, Y = y };
            Points.Add(point);
            Children.Add(point);
        }
    }
}

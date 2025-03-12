namespace PeterDoStuff.Graphics.Models
{
    public class PolygonModel : CompositeModel
    {
        public List<Model> Points => Children;

        public PolygonModel(double x1, double y1, double x2, double y2, double x3, double y3, params double[] coords)
            : this(
                  new Model() { X = x1, Y = y1 },
                  new Model() { X = x2, Y = y2 },
                  new Model() { X = x3, Y = y3 },
                  CreatePoints(coords))
        {
        }

        private static Model[] CreatePoints(double[] coords)
        {
            List<Model> points = [];
            for (int i = 0; i < coords.Length; i += 2)
            {
                var x = coords[i];
                var y = coords[i + 1];
                points.Add(new Model() { X = x, Y = y });
            }
            return [.. points];
        }

        public PolygonModel(Model point1, Model point2, Model point3, params Model[] points)
        {
            Points.Clear();
            Points.Add(point1);
            Points.Add(point2);
            Points.Add(point3);
            foreach (var point in points)
                Points.Add(point);
        }
    }
}

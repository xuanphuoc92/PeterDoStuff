using PeterDoStuff.Graphics.Effects;

namespace PeterDoStuff.Graphics.Models
{
    public class CurveModel : CompositeModel
    {
        public List<Model> Points => Children;

        public Model CurveTo(Model anchor, Model offset)
        {
            var stickTo = new StickTo(anchor);
            stickTo.Offset = offset;
            var point = new Model();
            point.Apply(stickTo);
            return CurveTo(point);
        }

        public Model CurveTo(Model point)
        {
            Add(point);
            return point;
        }

        public bool IsClosed = false;

        public void Close()
        {
            IsClosed = true;
        }
    }
}

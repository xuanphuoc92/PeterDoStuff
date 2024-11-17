namespace PeterDoStuff.Tools.Graphics
{
    public class SmoothCurve : CompositeModel
    {
        public SmoothCurve CurveTo(Model point)
        {
            Add(point);
            return this;
        }
    }
}

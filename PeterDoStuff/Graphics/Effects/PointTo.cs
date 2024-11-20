using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class PointTo : Effect
    {
        public PointTo(Model anchor)
        {
            Anchor = anchor;
        }

        public Model Anchor;

        /// <summary>
        /// The distance range where the rotation happens.
        /// If the distance of Model and Anchor is equal or less than this value, the rotation should not happen.
        /// Default as 0.
        /// </summary>
        public double Range = 0;

        public override void Tick()
        {
            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy);

            if (d > Range)
                Model.Deg = Math.Atan2(dy, dx).RadToDeg().Cap(-180, 180);
        }
    }
}

using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class RotateTo : Effect
    {
        public RotateTo(Model anchor)
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
            foreach (var model in Models)
            {
                var dx = Anchor.X - model.X;
                var dy = Anchor.Y - model.Y;
                var d = Math.Sqrt(dx * dx + dy * dy);

                if (d > Range)
                    model.Deg = Math.Atan2(dy, dx).RadToDeg().Cap(-180, 180);
            }
        }
    }
}

using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class PointTo : Effect
    {
        public PointTo(Model anchor, PointMode mode = PointMode.Toward)
        {
            Anchor = anchor;
            Mode = mode;
        }

        public Model Anchor;
        public PointMode Mode;
        public Model Offset = new();


        /// <summary>
        /// The distance range where the rotation happens.
        /// If the distance of Model and Anchor is equal or less than this value, the rotation should not happen.
        /// Default as 0.
        /// </summary>
        public double Range = 0;

        public override void Tick()
        {
            if (Mode == PointMode.Toward)
            {
                var dx = Anchor.X - Model.X;
                var dy = Anchor.Y - Model.Y;
                var d = Math.Sqrt(dx * dx + dy * dy);

                if (d > Range)
                    Model.Deg = (Math.Atan2(dy, dx).RadToDeg()+ Offset.Deg).Cap(-180, 180);
            }
            else
            {
                Model.Deg = (Anchor.Deg + Offset.Deg).Cap(-180, 180);
            }
        }
    }

    public enum PointMode
    {
        Toward = 0,
        Copy = 1,
    }
}

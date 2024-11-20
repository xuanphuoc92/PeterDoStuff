
using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class StickTo : Effect
    {
        public StickTo(double x, double y) : this(new Model() { X = x, Y = y })
        {
        }

        public StickTo(Model anchor)
        {
            Anchor = anchor;
        }

        public Model Offset = new();
        
        public Model Anchor;

        public override void Tick()
        {
            var rad = (Anchor.Deg + Offset.Deg).DegToRad();
            var dx = Anchor.Scale * (Offset.X * Math.Cos(rad) - Offset.Y * Math.Sin(rad));
            var dy = Anchor.Scale * (Offset.X * Math.Sin(rad) + Offset.Y * Math.Cos(rad));

            Model.X = Anchor.X + dx;
            Model.Y = Anchor.Y + dy;
        }
    }
}

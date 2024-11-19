
using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class MoveTo : Effect
    {
        public MoveTo(double x, double y) : this(new Model() { X = x, Y = y })
        {
        }

        public MoveTo(Model anchor)
        {
            Anchor = anchor;
        }

        public Model Offset = new();
        
        public Model Anchor;

        public override void Tick()
        {
            Models.ForEach(m =>
            {
                var rad = (Anchor.Deg + Offset.Deg).DegToRad();
                var dx = Anchor.Scale * (Offset.X * Math.Cos(rad) - Offset.Y * Math.Sin(rad));
                var dy = Anchor.Scale * (Offset.X * Math.Sin(rad) + Offset.Y * Math.Cos(rad));

                m.X = Anchor.X + dx;
                m.Y = Anchor.Y + dy;
            });
        }
    }
}

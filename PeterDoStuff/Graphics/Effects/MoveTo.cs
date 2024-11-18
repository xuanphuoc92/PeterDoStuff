
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
        
        public Model Anchor;

        public override void Tick()
        {
            Models.ForEach(m =>
            {
                m.X = Anchor.X;
                m.Y = Anchor.Y;
            });
        }
    }
}

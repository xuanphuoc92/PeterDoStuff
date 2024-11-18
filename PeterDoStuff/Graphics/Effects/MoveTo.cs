
namespace PeterDoStuff.Graphics.Effects
{
    public class MoveTo(Model anchor) : Effect
    {
        public Model Anchor = anchor;

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

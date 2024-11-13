
namespace PeterDoStuff.Tools.Graphics
{
    public class Wander : Animation
    {
        private Model Anchor;
        private Blink Blink;
        private Follow Follow;
        
        public Wander(double minX, double maxX, double minY, double maxY)
        {
            Blink = new(minX, maxX, minY, maxY);
            Anchor =  new();
            Anchor.AddAnimation(Blink);
            Follow = new(Anchor);
        }

        public TimeSpan Gap { get => Blink.BlinkGap; set => Blink.BlinkGap = value; }
        public double Velocity { get => Follow.Velocity; set => Follow.Velocity = value; }
        public double SlowRange { get => Follow.SlowRange; set => Follow.SlowRange = value; }

        protected override async Task Resolve(DateTime now)
        {
            if (Anchor.X == default & Anchor.Y == default)
            {
                Anchor.X = Model.X; Anchor.Y = Model.Y;
            }

            if (Follow.Model == null)
                Follow.Model = Model;

            await Anchor.Resolve(now);
            await Follow.Tick(now);
        }
    }
}

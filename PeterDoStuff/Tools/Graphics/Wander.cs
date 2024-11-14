namespace PeterDoStuff.Tools.Graphics
{
    public class Wander : Effect
    {
        public Model Anchor;
        public Blink Blink;
        public Follow Follow;
        
        public Wander(double minX, double maxX, double minY, double maxY)
        {
            Blink = new(minX, maxX, minY, maxY);
            Anchor =  new();
            Anchor.AddEffect(Blink);
            Follow = new(Anchor);
        }

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

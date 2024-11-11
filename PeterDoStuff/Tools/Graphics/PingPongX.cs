namespace PeterDoStuff.Tools.Graphics
{
    public class PingPongX(int Left, int Right, double Velocity = 1) : Animation
    {
        private bool leftToRight;

        public override async Task Tick(Model model, DateTime? now = null)
        {
            var timespan = UpdateTick(now);
            var delta = (timespan == null ? 1 : timespan.Value.TotalSeconds) * Velocity;

            if (model.X >= Right && leftToRight)
            {
                model.X = Right;
                leftToRight = !leftToRight;
            }
            else if (model.X <= Left && !leftToRight)
            {
                model.X = Left;
                leftToRight = !leftToRight;
            }

            model.X += (leftToRight ? 1 : -1) * delta;
        }
    }
}

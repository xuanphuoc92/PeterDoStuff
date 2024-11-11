namespace PeterDoStuff.Tools.Graphics
{
    public class PingPongX<TModel>(int Left, int Right, double Velocity = 1) : Animation<TModel>
        where TModel : Model
    {
        private bool leftToRight;

        public override async Task Tick(TModel model, DateTime? now = null)
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

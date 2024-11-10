namespace PeterDoStuff.Tools.Animations
{
    public class PingPongX<TModel>(int Left, int Right, int Velocity = 1) : Animation<TModel>
        where TModel : Model
    {
        private bool leftToRight;

        public override async Task Tick(TModel model)
        {
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
            model.X += leftToRight ? 1 : -1;
        }
    }
}

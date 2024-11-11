
namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingInBox
        (double Left, double Right, double Top, double Bottom, double VelocityX, double VelocityY) 
        : Animation
    {
        public override async Task Tick(Model model, DateTime? now = null)
        {
            var timespan = UpdateTick(now);

            double time = (timespan == null ? 1 : timespan.Value.TotalSeconds);

            var deltaX = time * VelocityX;
            var deltaY = time * VelocityY;

            model.X += deltaX;
            model.Y += deltaY;

            if (model.X < Left || model.X > Right)
                VelocityX *= -1;

            if (model.Y < Top || model.Y > Bottom)
                VelocityY *= -1;

            if (model.X < Left)
                model.X = Left;

            if (model.X > Right)
                model.X = Right;

            if (model.Y < Top)
                model.Y = Top;

            if (model.Y > Bottom)
                model.Y = Bottom;
        }
    }
}

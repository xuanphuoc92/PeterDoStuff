using PeterDoStuff.Extensions;

namespace PeterDoStuff.Tools.Graphics
{
    public class Rotating(double degreesPerSecond) : Effect
    {
        public double DegreesPerSecond = degreesPerSecond;

        protected override async Task Tick(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            var deltaDegress = DegreesPerSecond * timeSpan.TotalSeconds;
            Model.Degrees = (Model.Degrees + deltaDegress).Cap(0, 360);
        }
    }
}

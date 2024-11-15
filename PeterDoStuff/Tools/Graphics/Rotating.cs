using PeterDoStuff.Extensions;

namespace PeterDoStuff.Tools.Graphics
{
    public class Rotating : Effect
    {
        public double DegreesPerSecond;

        public Rotating(TimeSpan cycle)
            : this(360 / cycle.TotalSeconds)
        {
        }

        public Rotating(double degreesPerSecond)
        {
            DegreesPerSecond = degreesPerSecond;
        }

        protected override async Task Tick(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            var deltaDegress = DegreesPerSecond * timeSpan.TotalSeconds;
            Model.Degrees = (Model.Degrees + deltaDegress).Cap(0, 360);
        }
    }
}

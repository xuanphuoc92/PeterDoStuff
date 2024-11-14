namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Effect
    {
        public Model Model;
                
        public DateTime? LastTick;
        private void UpdateTick(DateTime now) => LastTick = now;

        protected TimeSpan FromLastTick(DateTime now)
            => LastTick == null ? TimeSpan.Zero : now - LastTick.Value;

        public async Task Tick(DateTime now)
        {
            await Resolve(now);
            UpdateTick(now);
        }

        protected abstract Task Resolve(DateTime now);

        protected double PointTo(double dx, double dy)
        {
            var degrees = Math.Atan2(dy, dx) * 180 / Math.PI;
            degrees += 90;
            return degrees % 360;
        }
    }

    public class CustomEffect : Effect
    {
        private Action<Model, DateTime> _animation;

        internal CustomEffect(Action<Model> animation) : this((t,m) => animation(t))
        {
        }

        internal CustomEffect(Action<Model, DateTime> animation)
        {
            _animation = animation;
        }

        protected override async Task Resolve(DateTime now)
        {
            _animation(Model, now);
        }
    }
}

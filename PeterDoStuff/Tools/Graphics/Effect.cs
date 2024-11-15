namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Effect
    {
        public Model Model;
                
        public DateTime? LastTick;
        private void UpdateTick(DateTime now) => LastTick = now;

        protected TimeSpan FromLastTick(DateTime now)
            => LastTick == null ? TimeSpan.Zero : now - LastTick.Value;

        public async Task Resolve(DateTime now)
        {
            await Tick(now);
            UpdateTick(now);
        }

        protected abstract Task Tick(DateTime now);
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

        protected override async Task Tick(DateTime now)
        {
            _animation(Model, now);
        }
    }
}

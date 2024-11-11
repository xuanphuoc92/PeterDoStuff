namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Animation
    {
        protected DateTime? LastTick { get; set; }
        protected TimeSpan? UpdateTick(DateTime? now)
        {
            if (now == null) return null;
            if (LastTick == null)
            {
                LastTick = now;
                return TimeSpan.Zero;
            }
            var timeSpan = now.Value - LastTick.Value;
            LastTick = now;
            return timeSpan;
        }

        public Task Tick(Model model, DateTime? now = null)
        {
            var timeSpan = UpdateTick(now);
            return Tick(model, timeSpan);
        }

        public abstract Task Tick(Model model, TimeSpan? timeSpan = null);
    }

    public class CustomAnimation : Animation
    {
        private Action<Model, TimeSpan?> _animation;

        internal CustomAnimation(Action<Model> animation) : this((t,m) => animation(t))
        {
        }

        internal CustomAnimation(Action<Model, TimeSpan?> animation)
        {
            _animation = animation;
        }

        public override async Task Tick(Model model, TimeSpan? timeSpan = null)
        {
            _animation(model, timeSpan);
        }
    }
}

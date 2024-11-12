namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Animation
    {
        public Model Model;
                
        public DateTime? LastTick;
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

        public Task Tick(DateTime? now = null)
        {
            var timeSpan = UpdateTick(now);
            return Tick(timeSpan);
        }

        public abstract Task Tick(TimeSpan? timeSpan = null);
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

        public override async Task Tick(TimeSpan? timeSpan = null)
        {
            _animation(Model, timeSpan);
        }
    }
}

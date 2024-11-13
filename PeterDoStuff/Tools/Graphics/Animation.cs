using System;

namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Animation
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

        public abstract Task Resolve(DateTime now);
    }

    public class CustomAnimation : Animation
    {
        private Action<Model, DateTime> _animation;

        internal CustomAnimation(Action<Model> animation) : this((t,m) => animation(t))
        {
        }

        internal CustomAnimation(Action<Model, DateTime> animation)
        {
            _animation = animation;
        }

        public override async Task Resolve(DateTime now)
        {
            _animation(Model, now);
        }
    }
}

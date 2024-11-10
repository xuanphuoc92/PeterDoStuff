namespace PeterDoStuff.Tools.Animations
{
    public interface Animation
    {
        Task Tick(Model model, DateTime? now = null);
    }
    

    public abstract class Animation<TModel> : Animation
        where TModel : Model
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

        public Task Tick(Model model, DateTime? now = null) => Tick((TModel)model, now);

        public abstract Task Tick(TModel model, DateTime? now = null);
    }

    public class CustomAnimation<TModel> : Animation<TModel>
        where TModel : Model
    {
        private Action<TModel, DateTime?> _animation;

        internal CustomAnimation(Action<TModel> animation) : this((t,m) => animation(t))
        {
        }

        internal CustomAnimation(Action<TModel, DateTime?> animation)
        {
            _animation = animation;
        }

        public override async Task Tick(TModel model, DateTime? now = null)
        {
            _animation(model, now);
        }
    }
}

namespace PeterDoStuff.Tools.Animations
{
    public interface Animation
    {
        Task Tick(Model model);
    }
    

    public abstract class Animation<TModel> : Animation
        where TModel : Model
    {
        public DateTime? LastTick { get; protected set; }

        public Task Tick(Model model) => Tick((TModel)model);

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

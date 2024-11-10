namespace PeterDoStuff.Tools.Animations
{
    public interface Animation
    {
        Task Tick(Model model);
    }
    

    public abstract class Animation<TModel> : Animation
        where TModel : Model
    {
        public Task Tick(Model model) => Tick((TModel)model);

        // Default: Do nothing.
        public abstract Task Tick(TModel model);
    }

    public class CustomAnimation<TModel>(Action<TModel> animation) : Animation<TModel>
        where TModel : Model
    {
        public override async Task Tick(TModel model)
        {
            animation(model);
        }
    }
}

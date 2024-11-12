namespace PeterDoStuff.Tools.Graphics
{
    public interface Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class Model : Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public async Task Tick(DateTime? now = null)
        {
            foreach (var animation in Animations)
                await animation.Tick(now);
        }

        public List<Animation> Animations = [];

        public string StrokeColor, FillColor;

        public double StrokeWidth = 1;

        public double StrokeOpacity = 1, FillOpacity = 0;

        public double Scale = 1;

        public double ScaledStrokeWidth => StrokeWidth * Scale;
    }

    public static class ModelExtensions
    {
        public static TModel ClearAnimations<TModel>(this TModel model)
            where TModel : Model
        {
            model.Animations.Clear();
            return model;
        }

        public static TModel AddAnimation<TModel>(this TModel model, Action<TModel> animation)
            where TModel : Model
        {
            CustomAnimation customAnimation = new CustomAnimation(m => animation((TModel)m));
            customAnimation.Model = model;
            model.Animations.Add(customAnimation);
            return model;
        }

        public static TModel AddAnimation<TModel>(this TModel model, Animation animation)
            where TModel : Model
        {
            animation.Model = model;
            model.Animations.Add(animation);
            return model;
        }
    }
}

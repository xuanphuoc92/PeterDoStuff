namespace PeterDoStuff.Tools.Graphics
{
    public interface Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public abstract class Model : Vector
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

        public static string DEFAULT_STROKE_COLOR = "#808080";
        public static double DEFAULT_STROKE_WIDTH = 1;
        public static double DEFAULT_STROKE_OPACITY = 1;

        public static string DEFAULT_FILL_COLOR = "#808080";
        public static double DEFAULT_FILL_OPACITY = 0;

        public static double DEFAULT_SCALE = 1;

        public string 
            StrokeColor = DEFAULT_STROKE_COLOR,
            FillColor = DEFAULT_FILL_COLOR;

        public double 
            StrokeWidth = DEFAULT_STROKE_WIDTH;

        public double 
            StrokeOpacity = DEFAULT_STROKE_OPACITY,
            FillOpacity = DEFAULT_FILL_OPACITY;

        public double Scale = DEFAULT_SCALE;

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

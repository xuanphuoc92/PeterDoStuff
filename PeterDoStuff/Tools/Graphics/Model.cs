using System.Text.Json.Serialization;

namespace PeterDoStuff.Tools.Graphics
{
    public abstract class Model
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Task Tick(DateTime? now = null) => Animation.Tick(this, now);

        [JsonIgnore]
        public Animation Animation { get; internal set; } = new DoNothing();

        public static string DEFAULT_STROKE_COLOR = "#808080";
        public static double DEFAULT_STROKE_WIDTH = 1;
        public static double DEFAULT_STROKE_OPACITY = 1;

        public static string DEFAULT_FILL_COLOR = "#808080";
        public static double DEFAULT_FILL_OPACITY = 0;

        public string StrokeColor { get; set; } = DEFAULT_STROKE_COLOR;
        public double StrokeWidth { get; set; } = DEFAULT_STROKE_WIDTH;
        public double StrokeOpacity { get; set; } = DEFAULT_STROKE_OPACITY;

        public string FillColor { get; set; } = DEFAULT_FILL_COLOR;
        public double FillOpacity { get; set; } = DEFAULT_FILL_OPACITY;

        public static double DEFAULT_SCALE = 1;
        public double Scale { get; set; } = DEFAULT_SCALE;
        public double ScaledStrokeWidth => StrokeWidth * Scale;
    }

    public static class ModelExtensions
    {
        public static TModel SetAnimation<TModel>(this TModel model, Action<TModel> animation)
            where TModel : Model
        {
            model.Animation = new CustomAnimation(m => animation((TModel)m));
            return model;
        }

        public static TModel SetAnimation<TModel>(this TModel model, Animation animation)
            where TModel : Model
        {
            model.Animation = animation;
            return model;
        }
    }
}

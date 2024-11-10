using System.Text.Json.Serialization;

namespace PeterDoStuff.Tools.Animations
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
        public static int DEFAULT_STROKE_WIDTH = 1;
        public static float DEFAULT_STROKE_OPACITY = 1;

        public static string DEFAULT_FILL_COLOR = "#808080";
        public static int DEFAULT_FILL_WIDTH = 0;
        public static float DEFAULT_FILL_OPACITY = 0;

        public string StrokeColor { get; set; } = DEFAULT_STROKE_COLOR;
        public int StrokeWidth { get; set; } = DEFAULT_STROKE_WIDTH;
        public float StrokeOpacity { get; set; } = DEFAULT_STROKE_OPACITY;

        public string FillColor { get; set; } = DEFAULT_FILL_COLOR;
        public int FillWidth { get; set; } = DEFAULT_FILL_WIDTH;
        public float FillOpacity { get; set; } = DEFAULT_FILL_OPACITY;
    }

    public static class ModelExtensions
    {
        public static TModel SetAnimation<TModel>(this TModel model, Action<TModel> animation)
            where TModel : Model
        {
            model.Animation = new CustomAnimation<TModel>(animation);
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

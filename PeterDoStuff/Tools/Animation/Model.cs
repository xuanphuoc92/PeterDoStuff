namespace PeterDoStuff.Tools.Animation
{
    public abstract class Model
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

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
}

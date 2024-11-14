namespace PeterDoStuff.Tools.Graphics
{
    public class Style
    {
        public string StrokeColor = "", FillColor = "";

        public double StrokeWidth = 1;

        public double StrokeOpacity = 1, FillOpacity = 0;

        public string StrokeDashArray = "";

        public Style Clone()
        {
            Style clone = new()
            {
                StrokeColor = StrokeColor,
                StrokeWidth = StrokeWidth,
                StrokeOpacity = StrokeOpacity,
                FillOpacity = FillOpacity,
                FillColor = FillColor,
            };
            return clone;
        }

        public Style SetStroke(
            string color, double width, double? opacity = null)
        {
            StrokeColor = color;
            StrokeWidth = width;
            if (opacity != null)
                StrokeOpacity = opacity.Value;
            return this;
        }

        public Style SetFill(
            string color, double? opacity = null)
        {
            FillColor = color;
            if (opacity != null)
                FillOpacity = opacity.Value;
            return this;
        }
    }
}

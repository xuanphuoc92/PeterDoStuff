namespace PeterDoStuff.Graphics
{
    public class Style
    {
        public string StrokeColor = "";
        public double StrokeWidth = 1, StrokeOpacity = 1;

        public string FillColor = "";
        public double FillOpacity = 0;

        public Style Clone()
        {
            Style clone = new()
            {
                StrokeColor = StrokeColor,
                StrokeWidth = StrokeWidth,
                StrokeOpacity = StrokeOpacity,
                FillColor = FillColor,
                FillOpacity = FillOpacity
            };
            return clone;
        }
    }
}

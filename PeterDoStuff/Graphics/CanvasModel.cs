using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.Graphics
{
    public class CanvasModel : CompositeModel
    {
        public RectangleModel Rect;
        public CanvasModel(double width, double height, Style? style = null)
        {
            if (style != null)
                Style = style;
            
            Rect = new RectangleModel(width, height);
            AddAndStyle(Rect);
        }
    }
}

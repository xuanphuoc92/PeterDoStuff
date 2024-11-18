using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.Graphics
{
    public class CanvasModel : CompositeModel
    {
        public string Name = "";
        public RectangleModel Rect;
        public Model Mouse = new();

        public CanvasModel(double width, double height, Style? style = null, Model? mouse = null)
        {
            if (style != null)
                Style = style;
            
            Rect = new RectangleModel(width, height);
            AddAndStyle(Rect);

            if (mouse != null)
                Mouse = mouse;
            AddAndStyle(Mouse);
        }
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class Canvas : CompositeModel
    {
        public string Name = "";
        public double Width => CanvasRect.Width;
        public double Height => CanvasRect.Height;

        public Rectangle CanvasRect;
        public Model Mouse;

        public Canvas(double width, double height, Style? style = null)
        {
            if (style != null)
                Style = style;
            CanvasRect = new Rectangle(width, height);
            AddAndStyle(CanvasRect);
            Mouse = new();
            Add(Mouse);
        }

        public Canvas AddAndStyle(Model model)
        {
            model.Style = Style.Clone();
            return Add(model);
        }

        public Canvas Add(Model model)
        {
            Models.Add(model);
            return this;
        }
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class Canvas
    {
        public Style Style = new();

        public string Name = "";
        public double Width => CanvasRect.Width;
        public double Height => CanvasRect.Height;

        public Canvas(double width, double height, Style? style = null)
        {
            if (style != null)
                Style = style;
            CanvasRect = new Rectangle(width, height)
            {
                Style = Style
            };
            Mouse = new();
        }

        public Rectangle CanvasRect;
        public Model Mouse;

        public List<Model> Models { get; private set; } = [];

        public Canvas AddModel(Model model)
        {
            model.Style = Style;
            Models.Add(model);
            return this;
        }

        public async Task<Canvas> Resolve()
        {
            DateTime now = DateTime.Now;
            foreach (var model in Models)
                await model.Resolve(now);
            await Mouse.Resolve(now);
            return this;
        }
    }
}

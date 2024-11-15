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

        public Task Resolve()
        {
            DateTime now = DateTime.Now;
            List<Task> tasks = [];
            foreach (var model in Models)
                tasks.Add(model.Resolve(now));
            tasks.Add(Mouse.Resolve(now));
            return Task.WhenAll(tasks);
        }
    }
}

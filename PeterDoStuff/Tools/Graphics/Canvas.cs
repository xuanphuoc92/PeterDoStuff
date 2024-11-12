namespace PeterDoStuff.Tools.Graphics
{
    public class Canvas
    {
        public ModelBuilder ModelBuilder = new();

        public string Name = "";
        public double Width => CanvasRect.Width;
        public double Height => CanvasRect.Height;

        public Canvas(double width, double height, ModelBuilder? builder = null)
        {
            if (builder != null)
                ModelBuilder = builder;
            CanvasRect = ModelBuilder.Build(() => new Rectangle(width, height));
        }

        public Rectangle CanvasRect { get; private set; }

        public List<Model> Models { get; private set; } = [];

        public Canvas AddModel(Model model)
        {
            model = ModelBuilder.Build(() => model);
            Models.Add(model);
            return this;
        }

        public async Task<Canvas> Resolve()
        {
            DateTime now = DateTime.Now;
            foreach (var model in Models)
                await model.Resolve(now);
            return this;
        }
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class Canvas
    {
        public ModelStyler ModelBuilder = new();

        public string Name = "";
        public double Width => CanvasRect.ScaledWidth;
        public double Height => CanvasRect.ScaledHeight;

        public Canvas(double width, double height, ModelStyler? builder = null)
        {
            if (builder != null)
                ModelBuilder = builder;
            CanvasRect = ModelBuilder.Style(() => new Rectangle(width, height));
        }

        public Rectangle CanvasRect { get; private set; }

        public List<Model> Models { get; private set; } = [];

        public Canvas AddAndStyleModel(Model model)
        {
            model = ModelBuilder.Style(() => model);
            return AddModel(model);
        }

        public Canvas AddModel(Model model)
        {
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

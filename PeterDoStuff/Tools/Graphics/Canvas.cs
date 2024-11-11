namespace PeterDoStuff.Tools.Graphics
{
    public class Canvas(double width, double height)
    {
        public double Width => CanvasRect.Width;
        public double Height => CanvasRect.Height;

        public Rectangle CanvasRect { get; private set; } = new Rectangle(width, height);

        public List<Model> Models { get; private set; } = [];

        public Canvas AddModel(Model model)
        {
            Models.Add(model);
            return this;
        }

        public async Task<Canvas> Tick()
        {
            DateTime now = DateTime.Now;
            foreach (var model in Models)
                await model.Tick(now);
            return this;
        }
    }
}

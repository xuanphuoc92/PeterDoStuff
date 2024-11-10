namespace PeterDoStuff.Tools.Animations
{
    public class Canvas(int width, int height)
    {
        public int Width 
        {
            get => CanvasRect.Width;
            set => CanvasRect.Width = value;
        }
        public int Height 
        {
            get => CanvasRect.Height;
            set => CanvasRect.Height = value;
        }

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

namespace PeterDoStuff.Tools.Animations
{
    public class Canvas(int Width, int Height)
    {
        public Rectangle CanvasRect { get; set; } = new Rectangle(Width, Height);

        public List<Model> Models { get; private set; } = [];

        public Canvas AddModel(Model model)
        {
            Models.Add(model);
            return this;
        }

        public async Task<Canvas> Tick()
        {
            foreach (var model in Models)
                await model.Tick();
            return this;
        }
    }
}

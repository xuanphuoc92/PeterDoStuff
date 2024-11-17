namespace PeterDoStuff.Tools.Graphics
{
    public class CompositeModel : Model
    {
        public List<Model> Models { get; } = [];

        public override async Task Resolve(DateTime? now = null)
        {
            await base.Resolve(now);
            List<Task> tasks = [];
            foreach (var model in Models)
                tasks.Add(model.Resolve(now));
            await Task.WhenAll(tasks);
        }

        public void AddAndStyle(params Model[] models)
        {
            foreach (var model in models)
                model.Style = Style.Clone();
            Add(models);
        }

        public void Add(params Model[] models)
        {
            foreach (var model in models)
            {
                model.Parent = this;
            }
            Models.AddRange(models);
        }
    }
}

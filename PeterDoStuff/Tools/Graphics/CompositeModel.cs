namespace PeterDoStuff.Tools.Graphics
{
    public class CompositeModel : Model
    {
        public List<Model> Models { get; } = [];

        public CompositeModel(params Model[] children)
        {
            Models.AddRange(children);
        }

        public override async Task Resolve(DateTime? now = null)
        {
            // To ensure all are synchronized.
            if (now == null)
                now = DateTime.Now;
            await base.Resolve(now);
            foreach (var child in Models)
                await child.Resolve(now);
        }
    }
}

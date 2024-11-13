namespace PeterDoStuff.Tools.Graphics
{
    public class CompositeModel : Model
    {
        public List<Model> Children { get; } = [];

        public CompositeModel(params Model[] children)
        {
            Children.AddRange(children);
        }

        public override async Task Resolve(DateTime? now = null)
        {
            await base.Resolve(now);
            foreach (var child in Children)
                await child.Resolve(now);
        }
    }
}

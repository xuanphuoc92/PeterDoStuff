
namespace PeterDoStuff.Graphics
{
    public class CompositeModel : Model
    {
        public List<Model> Children = [];

        public void AddAndStyle(params Model[] children)
        {
            foreach (var child in children)
                child.Style = Style.Clone();
            Add(children);
        }

        public void Add(params Model[] children)
        {
            Children.AddRange(children);
        }

        public override void Resolve(DateTime? now = null)
        {
            if (now == null)
                now = DateTime.Now;

            base.Resolve(now);
            foreach (var child in Children)
                child.Resolve(now);
        }
    }
}

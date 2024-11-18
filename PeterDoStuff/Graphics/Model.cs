namespace PeterDoStuff.Graphics
{
    public class Model
    {
        public double X, Y, Z, Deg;
        public double Scale = 1;

        public Style Style = new();
        
        public List<Effect> Effects = [];
        public void Apply(Effect effect)
        {
            effect.Models.Add(this);
            Effects.Add(effect);
        }

        public virtual void Resolve(DateTime? now = null)
        {
            if (now == null)
                now = DateTime.Now;
            Effects.ForEach(e => e.Resolve(now.Value));
        }
    }
}

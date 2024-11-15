namespace PeterDoStuff.Tools.Graphics
{
    public class Model
    {
        public double X, Y, Z;
        public double Degrees = 0;
        public double Scale = 1;
        public string SvgTransform => $"translate({X},{Y}) rotate({Degrees}) scale({Scale})";

        public List<Effect> Effects = [];
        public virtual async Task Resolve(DateTime? now = null)
        {
            if (now == null)
                now = DateTime.Now;
            foreach (var effect in Effects)
                await effect.Resolve(now.Value);
        }

        public Style Style = new();
    }

    public static class ModelExtensions
    {
        public static TModel ClearEffects<TModel>(this TModel model)
            where TModel : Model
        {
            model.Effects.Clear();
            return model;
        }

        public static TModel AddEffect<TModel>(this TModel model, Action<TModel> effect)
            where TModel : Model
        {
            CustomEffect customEffect = new(m => effect((TModel)m))
            {
                Model = model
            };
            model.Effects.Add(customEffect);
            return model;
        }

        public static TModel AddEffect<TModel>(this TModel model, Effect effect)
            where TModel : Model
        {
            effect.Model = model;
            model.Effects.Add(effect);
            return model;
        }
    }
}

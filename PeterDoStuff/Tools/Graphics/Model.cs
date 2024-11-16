using PeterDoStuff.Extensions;

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

        //public static TModel PointTo<TModel>(this TModel model, Model target)
        //    where TModel : Model
        //{
        //    var dx = target.X - model.X;
        //    var dy = target.Y - model.Y;
        //    return model.PointTo(dx, dy);
        //}

        public static TModel PointTo<TModel>(this TModel model, double dx, double dy)
            where TModel : Model
        {
            var degrees = Math.Atan2(dy, dx) * 180 / Math.PI;
            degrees += 90;
            model.Degrees =  degrees.Cap(-180, 180);
            return model;
        }

        public static TModel MoveTo<TModel>(this TModel model, Model target)
            where TModel : Model
        {
            return model.MoveTo(target.X, target.Y);
        }

        public static TModel MoveTo<TModel>(this TModel model, double x, double y)
            where TModel : Model
        {
            model.X = x;
            model.Y = y;
            return model;
        }

        public static TModel Move<TModel>(this TModel model, double dx, double dy)
            where TModel : Model
        {
            model.X += dx;
            model.Y += dy;
            return model;
        }

        public static (double d, double dx, double dy) GetDistance(this Model model, Model target)
        {
            double dx = target.X - model.X;
            double dy = target.Y - model.Y;
            double d = Math.Sqrt(dx * dx + dy * dy);
            return (d, dx, dy);
        }
    }
}

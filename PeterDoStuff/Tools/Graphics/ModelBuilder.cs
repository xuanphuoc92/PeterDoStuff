namespace PeterDoStuff.Tools.Graphics
{
    public class ModelBuilder
    {
        public string StrokeColor, FillColor;
        public double StrokeOpacity = 1, FillOpacity = 0;
        public double StrokeWidth = 1;

        public ModelBuilder SetStroke(
            string color, double width, double? opacity = null)
        {
            StrokeColor = color;
            StrokeWidth = width;
            if (opacity != null)
                StrokeOpacity = opacity.Value;
            return this;
        }

        public ModelBuilder SetFill(
            string color, double? opacity = null)
        {
            FillColor = color;
            if (opacity != null)
                FillOpacity = opacity.Value;
            return this;
        }

        public TModel Build<TModel>(Func<TModel> modelCreator) where TModel : Model
        {
            var model = modelCreator();
            model.StrokeColor = StrokeColor;
            model.StrokeOpacity = StrokeOpacity;
            model.StrokeWidth = StrokeWidth;
            model.FillColor = FillColor;
            model.FillOpacity = FillOpacity;
            return model;
        }
    }
}

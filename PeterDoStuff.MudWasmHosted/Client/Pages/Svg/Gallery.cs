using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class Gallery(Style style)
    {
        public CanvasModel[] Canvases = [
            new _01_BouncingBalls(style, new CircleModel(10))
        ];
    }
}

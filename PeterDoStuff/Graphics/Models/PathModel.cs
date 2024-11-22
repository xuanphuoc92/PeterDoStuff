using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Graphics.Models
{
    public class PathModel : Model
    {
        public List<Command> Commands = [];

        public void MoveTo(Model p)
        {
            Commands.Add(new Command() { Name = "M", Points = [p] });
        }

        public void CurveTo(Model c1, Model c2, Model p)
        {
            Commands.Add(new Command() { Name = "C", Points = [c1, c2, p] });
        }

        public class Command
        {
            public string Name;
            public List<Model> Points = [];
        }
    }
}

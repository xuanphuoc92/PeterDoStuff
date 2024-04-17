using PeterDoStuff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Tools.Schema
{
    public class Dependency
    {
        public Dependency(string leftString, string rightString)
        {
            LeftString = leftString;
            RightString = rightString;
        }

        public string LeftString
        {
            get => Left.Join(", ");
            set => Left = value.Split(",").Select(x => x.Trim()).ToList();
        }
        public string RightString
        {
            get => Right.Join(", ");
            set => Right = value.Split(", ").Select(x => x.Trim()).ToList();
        }

        public List<string> Left { get; private set; } = new List<string>();
        public List<string> Right { get; private set; } = new List<string>();
    }
}

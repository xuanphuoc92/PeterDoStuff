using PeterDoStuff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Tools
{
    public class SchemaNormalizer
    {
        public List<string> GetAllAttributes()
        {
            var allLefts = Dependencies.SelectMany(d => d.Left);
            var allRights = Dependencies.SelectMany(d => d.Right);
            return allLefts.Union(allRights).Distinct().ToList();
        }

        public static SchemaNormalizer New()
        {
            return new SchemaNormalizer();
        }

        public SchemaNormalizer AddDependency(string[] left, string[] right)
        {
            Dependencies.Add(new Dependency() { LeftString = left.Join(", "), RightString = right.Join(", ") });
            return this;
        }

        public List<Dependency> Dependencies { get; private set; } = new List<Dependency>();

        public class Dependency
        {
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
            
            //public bool IsTrivial()
            //{
            //    foreach (var right in Right)
            //    {
            //        if (!Left.Contains(right))
            //            return false;
            //    }
            //    return true;
            //}
        }
    }
}

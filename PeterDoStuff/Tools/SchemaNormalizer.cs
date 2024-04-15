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
            var allLefts = FuncDependencies.SelectMany(d => d.Left);
            var allRights = FuncDependencies.SelectMany(d => d.Right);
            return allLefts.Union(allRights).Distinct().ToList();
        }

        public static SchemaNormalizer New()
        {
            return new SchemaNormalizer();
        }

        public SchemaNormalizer AddFuncDependency(string[] left, string[] right)
        {
            FuncDependencies.Add(new FuncDependency() { LeftString = left.Join(", "), RightString = right.Join(", ") });
            return this;
        }

        public List<FuncDependency> FuncDependencies { get; private set; } = new List<FuncDependency>();

        public struct FuncDependency
        {
            public FuncDependency() { }

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

            public bool IsTrivial()
            {
                foreach (var right in Right)
                {
                    if (!Left.Contains(right))
                        return false;
                }
                return true;
            }

            public bool IsNonTrivial() 
                => IsTrivial() == false;

            public bool IsCompletelyNonTrivial()
            {
                if (IsTrivial() == true) return false;

                foreach (var right in Right)
                    if (Left.Contains(right)) return false;
                
                return true;
            }
        }
    }
}

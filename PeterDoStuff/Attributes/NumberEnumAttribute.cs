using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Attributes
{
    /// <summary>
    /// This enum attribute indicates that when saved in the database, the enum value is turned into int type.
    /// </summary>
    public class NumberEnumAttribute : Attribute { }
}

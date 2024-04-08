using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Attributes
{
    /// <summary>
    /// This class helps migrator to create a cloned [{TableName}_Deleted] table in its migration script (without Primary Key constraint)
    /// </summary>
    public class WithDeletedBinAttribute : Attribute
    {
    }
}

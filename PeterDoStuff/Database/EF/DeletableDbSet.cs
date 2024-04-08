using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Database.EF
{
    public interface IDbSetContainer<TEntity> where TEntity : class
    {
        public DbSet<TEntity> Main { get; set; }
    }

    public class DeletableDbSet<TEntity> : IDbSetContainer<TEntity>
        where TEntity : class
    {
        public DbSet<TEntity> Main { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MetadataService.Model;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace MetadataService.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("databaseContext") { }
            public DbSet<Argument> Arguments { get; set; }
            public DbSet<Method> Methods { get; set; }
            public DbSet<Package> Packages { get; set; }

            public ObjectContext ObjectContext
            {
                get { return ((IObjectContextAdapter)this).ObjectContext; }
            }

    }
}

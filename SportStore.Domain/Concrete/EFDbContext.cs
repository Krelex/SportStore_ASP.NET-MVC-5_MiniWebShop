using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportStore.Domain.Entities;
using System.Data.Entity;

namespace SportStore.Domain.Concrete
{
    class EFDbContext : DbContext
    {
        public EFDbContext() 
        : base("EFDbContext") 
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}

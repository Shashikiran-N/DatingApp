using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        // Need this constructor
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // Creates a table named Users
        public DbSet<AppUser> Users { get; set; }
    }
}

// Next add this configuration inside the startup class so that datacontext can be injected
// to other parts of application
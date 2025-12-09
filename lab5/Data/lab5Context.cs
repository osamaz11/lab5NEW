using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using lab5.Models;
using lab5;

namespace lab5.Data
{
    public class lab5Context : DbContext
    {
        public lab5Context (DbContextOptions<lab5Context> options)
            : base(options)
        {
        }

        public DbSet<lab5.Models.book> book { get; set; } = default!;
        public DbSet<lab5.orderdetail> orderdetail { get; set; } = default!;
        public DbSet<lab5.Models.usersaccounts> usersaccounts { get; set; } = default!;
        public DbSet<lab5.Models.orders> orders { get; set; } = default!;
        public DbSet<lab5.Models.bookorder> bookorder { get; set; } = default!;
        public DbSet<lab5.Models.orderline> orderline { get; set; } = default!;

    }
}

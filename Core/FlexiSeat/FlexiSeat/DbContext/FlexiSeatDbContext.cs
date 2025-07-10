using FlexiSeat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;

namespace FlexiSeat.DbContext
{
    public class FlexiSeatDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public FlexiSeatDbContext(DbContextOptions<FlexiSeatDbContext> options) : base(options) { }

        public DbSet<AppRole> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<UserLogin> UserLogins { get; set; }
        //public DbSet<Report> Reports { get; set; }
        //public DbSet<Designation> Designations { get; set; }

    }
}

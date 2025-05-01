
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QueueOptimizer.Models;

namespace QueueOptimizer.Data;

public class AppDbContext : IdentityDbContext<Users>
{
    public DbSet<Table> Tables { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Table>().HasData(
         
            new Table { Id = 1, Seats = 1, PositionX = 50, PositionY = 50 },
            new Table { Id = 2, Seats = 1, PositionX = 120, PositionY = 50 },
            new Table { Id = 3, Seats = 1, PositionX = 190, PositionY = 50 },
            new Table { Id = 4, Seats = 1, PositionX = 260, PositionY = 50 },
            new Table { Id = 5, Seats = 1, PositionX = 50, PositionY = 120 },
            new Table { Id = 6, Seats = 1, PositionX = 50, PositionY = 190 },
            new Table { Id = 7, Seats = 1, PositionX = 50, PositionY = 260 },

         
            new Table { Id = 8, Seats = 2, PositionX = 150, PositionY = 600 },
            new Table { Id = 9, Seats = 2, PositionX = 250, PositionY = 600 },
            new Table { Id = 10, Seats = 2, PositionX = 350, PositionY = 600 },
            new Table { Id = 11, Seats = 2, PositionX = 450, PositionY = 600 },
            new Table { Id = 12, Seats = 2, PositionX = 550, PositionY = 600 },
            new Table { Id = 13, Seats = 2, PositionX = 650, PositionY = 600 },

            new Table { Id = 14, Seats = 3, PositionX = 200, PositionY = 500 },
            new Table { Id = 15, Seats = 3, PositionX = 350, PositionY = 500 },
            new Table { Id = 16, Seats = 3, PositionX = 500, PositionY = 500 },
            new Table { Id = 17, Seats = 3, PositionX = 650, PositionY = 500 },

          
            new Table { Id = 18, Seats = 6, PositionX = 200, PositionY = 300 },
            new Table { Id = 19, Seats = 6, PositionX = 400, PositionY = 300 },
            new Table { Id = 20, Seats = 6, PositionX = 600, PositionY = 300 }
        );
    }


    public DbSet<Reservation> Reservations { get; set; }

}

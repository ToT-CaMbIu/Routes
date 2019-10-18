using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;

namespace Routes.Models
{
    public class RouteContext : IdentityDbContext<User, Role, int>
    {
        public RouteContext(DbContextOptions<RouteContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder
                 .Entity<User>()
                 .Property(p => p.Role)
                 .HasConversion(
                     v => v.ToString(),
                     v => (RoleType)Enum.Parse(typeof(RoleType), v));*/
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }

    public class User : IdentityUser<int>
    {
        public string Nickname { get; set; }
        public string Country { get; set; }
        public double Rating { get; set; }
        public string InfoAbout { get; set; }
        public bool isPremium { get; set; }

        public List<Route> Routes { get; set; }
    }

    public class Role : IdentityRole<int>
    {
    }

    public class Place
    {
        public double Lt { get; set; }
        public double Lg { get; set; }
        public string InfoAbout { get; set; }
        public int Id { get; set; }

        [ForeignKey("RouteId")]
        public Route Route { get; set; }
        public int RouteId { get; set; }
    }

    public class Route
    {
        public string Name { get; set; }
        public int CountOfViews { get; set; }
        public int Id { get; set; }
        public bool isForPremium { get; set; }

        [ForeignKey("RouteUserId")]
        public User RouteCreator { get; set; }
        public int RouteUserId { get; set; }

        public List<Place> Places { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public string Content { get; set; }
        public int Id { get; set; }

        [ForeignKey("СommentUserId")]
        public User CommentCreator { get; set; }
        public int CommentUserId { get; set; }

        [ForeignKey("RouteId")]
        public int RouteId { get; set; }
        public Route Route { get; set; }
    }
}

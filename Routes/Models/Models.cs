using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;

namespace Routes.Models
{
    public class RouteContext : DbContext
    {
        public RouteContext(DbContextOptions<RouteContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
    }

    public class User
    {
        public string Nickname { get; set; }
        public string Country { get; set; }
        public double Rating { get; set; }
        public string InfoAbout { get; set; }
        public bool isPremium { get; set; }
        public int Id { get; set; }
        public string Role { get; set; }
    }

    public class Place
    {
        public double Lt { get; set; }
        public double Lg { get; set; }
        public string InfoAbout { get; set; }
        public int Id { get; set; }
    }

    public class Route
    {
        public double Rating { get; set; }
        public User Creator { get; set; }
        public int Id { get; set; }
        public List<Place> Places { get; set; }
        public List<Comment> Comments { get; set; }
        public List<User> Evaluators { get; set; }
        public bool isForPremium { get; set; }
    }

    public class Comment
    {
        public User Creator { get; set; }
        public List<Reply> Replies { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
    }

    public class Reply
    {
        public User Creator { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
    }
}

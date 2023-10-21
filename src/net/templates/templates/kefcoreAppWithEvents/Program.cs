﻿using MASES.EntityFrameworkCore.KNet.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MASES.EntityFrameworkCore.KNet.Templates
{
    partial class Program
    {
        static void OnEvent(IEntityType entity, bool state, object key)
        {
            Console.WriteLine($"Entity {entity.Name} has {(state ? "removed" : "added/updated")} the key {key}");
        }

        static void Main(string[] args)
        {
            BloggingContext context = null;
            try
            {
                context = new BloggingContext()
                {
                    BootstrapServers = "KAFKA-BROKER:9092",
                    ApplicationId = "MyApplicationId",
                    DatabaseName = "MyDB",
                    OnChangeEvent = OnEvent
                };
                // cleanup topics on Broker
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // prefill data
                for (int i = 0; i < 1000; i++)
                {
                    context.Add(new Blog
                    {
                        Url = "http://blogs.msdn.com/adonet" + i.ToString(),
                        Posts = new List<Post>()
                            {
                                new Post()
                                {
                                    Title = "title",
                                    Content = i.ToString()
                                }
                            },
                        Rating = i,
                    });
                }
                // save data
                context.SaveChanges();

                // make some queries
                var selector = (from op in context.Blogs
                                join pg in context.Posts on op.BlogId equals pg.BlogId
                                where pg.BlogId == op.BlogId
                                select new { pg, op });
                var pageObject = selector.FirstOrDefault();

                var post = context.Posts.Single(b => b.BlogId == 2);

                post = context.Posts.Single(b => b.BlogId == 1);

                var all = context.Posts.All((o) => true);

                var value = context.Blogs.AsQueryable().ToQueryString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                context?.Dispose();
            }
        }
    }

    public class BloggingContext : KafkaDbContext
    {
        // uncomment for persistent storage
        // public override bool UsePersistentStorage { get; set; } = true;

        // uncomment to disable compacted replicator
        //public override bool UseCompactedReplicator { get; set; } = false;

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        /// uncomment for model builder
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blog>().HasKey(c => new { c.BlogId, c.Rating });
        //}
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }

        public override string ToString()
        {
            return $"BlogId: {BlogId} Url: {Url} Rating: {Rating}";
        }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public override string ToString()
        {
            return $"PostId: {PostId} Title: {Title} Content: {Content} BlogId: {BlogId}";
        }
    }
}

using Wyklad10Test.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wyklad10Test.Data
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Tools" },
                new Category { CategoryId = 2, CategoryName = "Vegetables" }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article(1, "Hammer", 50, 1, null),
                new Article(2, "Carrot", 2, 2, null)
            );
        }

    }
}

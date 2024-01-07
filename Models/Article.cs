using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wyklad10Test.Models
{
    public class Article
    {
        public int ArticleId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Too short name")]
        [MaxLength(50, ErrorMessage = "Too long name, do not exceed {0}")]
        public string ArticleName { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Price must be between {1} and {2}")]
        public int Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string ImagePath { get; set; }

        public Article() { }

        public Article(int articleId, string articleName, int price, int categoryId, string imagePath)
        {
            this.ArticleId = articleId;
            this.ArticleName = articleName;
            this.Price = price;
            this.CategoryId = categoryId;
            this.ImagePath = imagePath;
        }
    }
}

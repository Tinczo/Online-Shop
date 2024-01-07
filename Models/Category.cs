using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wyklad10Test.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = " To long name, do not exceed {0}")]
        public string CategoryName { get; set; }
        public List<Article> Articles { get; set; } = new List<Article>();

    }
}

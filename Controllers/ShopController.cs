using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppForEntityFrameworkDemo.Data;
using Wyklad10Test.Models;

namespace Wyklad10Test.Controllers
{
    public class ShopController : Controller
    {
        private readonly MyDbContext _context;

        public ShopController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Category.ToListAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int categoryId)
        {
            return RedirectToAction("Display", new { categoryId = categoryId });
        }

        // GET: Shop/Display/5
        public async Task<IActionResult> Display(int? categoryId)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            var articles = await _context.Article
                .Include(a => a.Category)
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();

            if (!articles.Any())
            {
                return NotFound();
            }

            return View(articles);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebAppForEntityFrameworkDemo.Data;
using Wyklad10Test.Models;
using Wyklad10Test.ViewModels;

namespace Wyklad10Test.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ArticlesController> _logger;
        private readonly IHostingEnvironment _hostingEnviroment;

        public ArticlesController(MyDbContext context, ILogger<ArticlesController> logger, IHostingEnvironment hostingEnviroment)
        {
            _context = context;
            _logger = logger;
            _hostingEnviroment = hostingEnviroment;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Article.Include(a => a.Category);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        public async Task<IActionResult> Cart()
        {
            var cartItems = new List<CartItemViewModel>();
            decimal total = 0m;

            foreach (var cookie in Request.Cookies)
            {
                if (int.TryParse(cookie.Key, out int articleId) && int.TryParse(cookie.Value, out int quantity))
                {
                    var article = await _context.Article
                        .Include(a => a.Category)
                        .FirstOrDefaultAsync(a => a.ArticleId == articleId);
                    if (article != null && quantity > 0)
                    {
                        cartItems.Add(new CartItemViewModel
                        {
                            Article = article,
                            Quantity = quantity
                        });
                        total += article.Price * quantity;
                    }
                }
            }

            ViewBag.Total = total;

            var sortedCartItems = cartItems.OrderBy(c => c.Article.ArticleId).ToList();

            return View(sortedCartItems);
        }

        public async Task<IActionResult> Summary()
        {
            var cartItems = new List<CartItemViewModel>();
            decimal total = 0m;

            foreach (var cookie in Request.Cookies)
            {
                if (int.TryParse(cookie.Key, out int articleId) && int.TryParse(cookie.Value, out int quantity))
                {
                    var article = await _context.Article
                        .Include(a => a.Category)
                        .FirstOrDefaultAsync(a => a.ArticleId == articleId);
                    if (article != null && quantity > 0)
                    {
                        cartItems.Add(new CartItemViewModel
                        {
                            Article = article,
                            Quantity = quantity
                        });
                        total += article.Price * quantity;
                    }
                }
            }

            ViewBag.Total = total;

            var sortedCartItems = cartItems.OrderBy(c => c.Article.ArticleId).ToList();

            return View(sortedCartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                return View("ArticleDeleted");
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                //return NotFound();
                return View("ArticleDeleted");
            }

            string cookieKey = $"{id}";
            int currentCount = 0;

            if (Request.Cookies[cookieKey] != null)
            {
                if (int.TryParse(Request.Cookies[cookieKey], out currentCount))
                {
                    currentCount++;
                }
            }
            else
            {
                currentCount = 1;
            }

            SetCookie(cookieKey, currentCount.ToString());
            return RedirectToAction(nameof(Index));
        }


        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ArticleName,Price,CategoryId,FormFile")] ViewModelArticle viewModelArticle)
        {
            if (ModelState.IsValid)
            {
                if (viewModelArticle.FormFile != null)
                {
                    var filename = Guid.NewGuid().ToString() + viewModelArticle.FormFile.FileName;
                    var filePath = Path.Combine(_hostingEnviroment.WebRootPath, "upload") + "\\" + filename;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        viewModelArticle.FormFile.CopyTo(stream);
                        viewModelArticle.ImagePath = Path.Combine("upload") + "\\" + filename;
                    }
                }

                Article article = new Article(
                    viewModelArticle.ArticleId,
                    viewModelArticle.ArticleName,
                    viewModelArticle.Price,
                    viewModelArticle.CategoryId,
                    viewModelArticle.ImagePath);
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", viewModelArticle.CategoryId);
            return View(viewModelArticle);

        }
            
        

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.Article.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Price,CategoryId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArticle = await _context.Article.FindAsync(id);

                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    existingArticle.ArticleName = article.ArticleName;
                    existingArticle.Price = article.Price;
                    existingArticle.CategoryId = article.CategoryId;

                    _context.Update(existingArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            var filePath = Path.Combine(_hostingEnviroment.WebRootPath) + "\\" + article.ImagePath;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IncrementCartItem(int articleId)
        {
            var quantity = GetCookieValue(articleId.ToString()) ?? 0;
            if (quantity > 0)
                SetCookie(articleId.ToString(), (quantity + 1).ToString());
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecrementCartItem(int articleId)
        {
            var quantity = GetCookieValue(articleId.ToString()) ?? 0;
            if (quantity > 0)
                SetCookie(articleId.ToString(), (quantity - 1).ToString());
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCartItem(int articleId)
        {
            SetCookie(articleId.ToString(), "0"); 
            return RedirectToAction("Cart");
        }

        private int? GetCookieValue(string key)
        {
            if (Request.Cookies.TryGetValue(key, out string value))
            {
                if (int.TryParse(value, out int intValue))
                {
                    return intValue;
                }
            }
            return null;
        }


        public void SetCookie(string key, string value, int? numberOfSeconds = 604800)
        {
            CookieOptions option = new CookieOptions();
            if (numberOfSeconds.HasValue)
                option.Expires = DateTime.Now.AddSeconds(numberOfSeconds.Value);
            Response.Cookies.Append(key, value, option);
        }

    }
}

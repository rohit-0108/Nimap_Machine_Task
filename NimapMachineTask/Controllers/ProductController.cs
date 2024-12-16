using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NimapMachineTask.Data;
using NimapMachineTask.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NimapMachineTask.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var totalProducts = _context.Products.Count();

            // Calculate the total number of pages
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Fetch the records for the current page
            var products = _context.Products
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize) // Skip previous pages' records
                .Take(pageSize)             // Take records for the current page
                .ToList();

            // Create the view model with pagination data
            var model = new PagedResult
            {
                Products = products,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalProducts
            };

            return View(model);

        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }
            Console.WriteLine($"CategoryId: {product.CategoryId}");

            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        
           
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            ViewBag.Categories = _context.Categories.Select(
                c=>new SelectListItem
                {
                    Value=c.CategoryId.ToString(),
                    Text=c.CategoryName
                }).ToList();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.Select(
                    c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                return View(product);
            }
            var existingProduct = _context.Products.Find(product.ProductId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update product fields
            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryId = product.CategoryId;

            // Save changes
            _context.SaveChanges();

            // Redirect back to the index view
            return RedirectToAction(nameof(Index));
        }


        //// GET Edit
        //public IActionResult Edit(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    if (product == null) return NotFound();

        //    // Load categories for the dropdown
        //    ViewBag.Categories = _context.Categories.ToList();
        //    return View(product);
        //}

        //// POST Edit
        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Reload categories to display in the dropdown in case of validation error
        //        ViewBag.Categories = _context.Categories.ToList();
        //        return View(product);
        //    }

        //    // Ensure the product exists in the database before updating
        //    var existingProduct = _context.Products.Find(product.ProductId);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update product fields
        //    existingProduct.ProductName = product.ProductName;
        //    existingProduct.CategoryId = product.CategoryId;

        //    // Save changes
        //    _context.SaveChanges();

        //    // Redirect back to the index view
        //    return RedirectToAction(nameof(Index));
        //}



        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

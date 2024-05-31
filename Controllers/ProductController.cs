using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgriEnergy.Models;
using AgriEnergy.ViewModels;

namespace AgriEnergy.Controllers
{
    public class ProductController : Controller
    {
        private readonly AgriEnergyContext _context;

        public ProductController(AgriEnergyContext context)
        {
            _context = context;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's UserUid
            var token = HttpContext.Session.GetString("currentUser");

            if (string.IsNullOrEmpty(token))
            {
                // Redirect to login if not logged in
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve products associated with the logged-in user, assuming UserRole 1 is for farmers
            var products = await _context.Products
                                        .Include(p => p.User)
                                        .Where(p => p.User.UserUid == token)
                                        .ToListAsync();

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Category,ProductionDate,UserId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", product.UserId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", product.UserId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Category,ProductionDate,UserId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", product.UserId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // GET: Product/Employee
        public async Task<IActionResult> Employee()
        {
            var farmers = await _context.Users
                                        .Where(u => u.UserRole == 1)
                                        .ToListAsync();

            var categories = await _context.Products
                                           .Select(p => p.Category)
                                           .Distinct()
                                           .ToListAsync();

            var viewModel = new EmployeeViewModel
            {
                Farmers = farmers,
                Categories = categories,
                Products = Enumerable.Empty<Product>()
            };

            return View(viewModel);
        }

        // POST: Product/Employee
        [HttpPost]
        public async Task<IActionResult> Employee(EmployeeViewModel viewModel)
        {
            viewModel.Farmers = await _context.Users
                                              .Where(u => u.UserRole == 1)
                                              .ToListAsync();

            viewModel.Categories = await _context.Products
                                                 .Select(p => p.Category)
                                                 .Distinct()
                                                 .ToListAsync();

            var query = _context.Products.AsQueryable();

            if (viewModel.SelectedFarmerId != 0)
            {
                query = query.Where(p => p.UserId == viewModel.SelectedFarmerId);
            }

            if (!string.IsNullOrEmpty(viewModel.SelectedCategory))
            {
                query = query.Where(p => p.Category == viewModel.SelectedCategory);
            }

            viewModel.Products = await query.ToListAsync();

            return View(viewModel);
        }
    }
}

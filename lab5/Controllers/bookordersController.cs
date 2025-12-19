using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab5.Data;
using lab5.Models;
using System.Text.Json;

namespace lab5.Controllers
{
    public class bookordersController : Controller
    {
        private readonly lab5Context _context;
        private List<buybook> Bbks = new List<buybook>();

        public bookordersController(lab5Context context)
        {
            _context = context;
        }

        // =========================
        // ADD TO CART
        // =========================
        [HttpPost]
        public async Task<IActionResult> cartadd(int bookId, int quantity)
        {
            await HttpContext.Session.LoadAsync();

            var book = await _context.book.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
                return NotFound();

            // 🔴 STOCK VALIDATION
            if (quantity > book.bookquantity)
            {
                TempData["Error"] =
                    $"Only {book.bookquantity} copies of '{book.title}' are available.";
                return RedirectToAction("CatalogueBuy", "orders");
            }

            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString != null)
            {
                Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);
            }

            Bbks.Add(new buybook
            {
                Title = book.title,
                Price = book.price,
                quant = quantity
            });

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));
            return RedirectToAction("CartBuy");
        }

        // =========================
        // VIEW CART
        // =========================
        public async Task<IActionResult> CartBuy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString != null)
            {
                Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);
            }
            return View(Bbks);
        }

        // =========================
        // BUY / CHECKOUT
        // =========================
        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString == null)
                return RedirectToAction("CartBuy");

            Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);

            string ctname = HttpContext.Session.GetString("Name");

            bookorder bkorder = new bookorder
            {
                custname = ctname,
                orderdate = DateTime.Today,
                total = 0
            };

            _context.bookorder.Add(bkorder);
            await _context.SaveChangesAsync();

            decimal total = 0;

            foreach (var bk in Bbks)
            {
                var book = await _context.book.FirstOrDefaultAsync(b => b.title == bk.Title);

                // 🔴 FINAL STOCK VALIDATION
                if (book == null || bk.quant > book.bookquantity)
                {
                    TempData["Error"] =
                        $"Not enough stock for '{bk.Title}'. Available: {book?.bookquantity ?? 0}";
                    return RedirectToAction("CartBuy");
                }

                orderline line = new orderline
                {
                    orderid = bkorder.Id,
                    itemname = bk.Title,
                    itemquant = bk.quant,
                    itemprice = bk.Price
                };

                _context.orderline.Add(line);

                // Update stock
                book.bookquantity -= bk.quant;
                _context.book.Update(book);

                total += bk.quant * bk.Price;
            }

            bkorder.total = Convert.ToInt32(total);
            _context.bookorder.Update(bkorder);

            await _context.SaveChangesAsync();

            // Clear cart
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(new List<buybook>()));

            return RedirectToAction("MyOrder");
        }

        // =========================
        // MY ORDERS
        // =========================
        public async Task<IActionResult> MyOrder()
        {
            string ctname = HttpContext.Session.GetString("Name");
            return View(await _context.bookorder
                .Where(o => o.custname == ctname)
                .ToListAsync());
        }

        public async Task<IActionResult> Orderline(int? orid)
        {
            return View(await _context.orderline
                .Where(o => o.orderid == orid)
                .ToListAsync());
        }

        // =========================
        // ADMIN CRUD
        // =========================
        public async Task<IActionResult> Index()
        {
            return View(await _context.bookorder.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder == null) return NotFound();

            return View(bookorder);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(bookorder bookorder)
        {
            if (!ModelState.IsValid) return View(bookorder);

            _context.bookorder.Add(bookorder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder == null) return NotFound();

            return View(bookorder);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, bookorder bookorder)
        {
            if (id != bookorder.Id) return NotFound();

            if (!ModelState.IsValid) return View(bookorder);

            _context.Update(bookorder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder == null) return NotFound();

            return View(bookorder);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder != null)
            {
                _context.bookorder.Remove(bookorder);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

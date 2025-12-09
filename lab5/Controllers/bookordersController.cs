using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab5.Data;
using lab5.Models;
using System.Text.Json;


namespace lab5.Controllers
{
    public class bookordersController : Controller
    {
        List<buybook> Bbks = new List<buybook>();

        private readonly lab5Context _context;

        public bookordersController(lab5Context context)
        {
            _context = context;
        }

        // GET: bookorders
        [HttpPost]
        public async Task<IActionResult> cartadd(int bookId, int quantity)
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);
            }

            var book = await _context.book.FromSqlRaw("select * from book  where Id= '" + bookId + "'  ").FirstOrDefaultAsync();

            Bbks.Add(new buybook
            {
                Title = book.title,
                Price = book.price,
                quant = quantity
            });

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));
            return RedirectToAction("CartBuy");
        }



        public async Task<IActionResult> CartBuy()
        {

            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);
            }
            return View(Bbks);
        }
        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bbks = JsonSerializer.Deserialize<List<buybook>>(sessionString);
            }

            string ctname = HttpContext.Session.GetString("Name");
            bookorder bkorder = new bookorder();
            bkorder.total = 0;
            bkorder.custname = ctname;
            bkorder.orderdate = DateTime.Today;
            _context.bookorder.Add(bkorder);
            await _context.SaveChangesAsync();
            var bord = await _context.bookorder.FromSqlRaw("select * from bookorder  where custname= '" + ctname + "' ").OrderByDescending(e => e.Id).FirstOrDefaultAsync();
            int ordid = bord.Id;
            decimal tot = 0;
            foreach (var bk in Bbks.ToList())
            {
                orderline oline = new orderline();
                oline.orderid = ordid;
                oline.itemname = bk.Title;
                oline.itemquant = bk.quant;
                oline.itemprice = bk.Price;
                _context.orderline.Add(oline);
                await _context.SaveChangesAsync();

                var bkk = await _context.book.FromSqlRaw("select * from book  where title= '" + bk.Title + "' ").FirstOrDefaultAsync();
                bkk.bookquantity = bkk.bookquantity - bk.quant;
                _context.Update(bkk);
                await _context.SaveChangesAsync();

                tot = tot + (bk.quant * bk.Price);
            }
            bord.total = Convert.ToInt16(tot);
            _context.Update(bord);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Thank you See you again";
            Bbks = new List<buybook>();
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));
            return RedirectToAction("MyOrder");
        }
        public async Task<IActionResult> MyOrder()
        {
            string ctname = HttpContext.Session.GetString("Name");
            return View(await _context.bookorder.FromSqlRaw("select * from bookorder  where custname = '" + ctname + "' ").ToListAsync());
        }
        public async Task<IActionResult> Orderline(int? orid)
        {
            var buybk = await _context.orderline.FromSqlRaw("select * from orderline  where orderid = '" + orid + "' ").ToListAsync();
            return View(buybk);
        }



        public async Task<IActionResult> Index()
        {
            return View(await _context.bookorder.ToListAsync());
        }

        // GET: bookorders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookorder = await _context.bookorder
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookorder == null)
            {
                return NotFound();
            }

            return View(bookorder);
        }

        // GET: bookorders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: bookorders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,custname,total,orderdate")] bookorder bookorder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookorder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookorder);
        }

        // GET: bookorders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder == null)
            {
                return NotFound();
            }
            return View(bookorder);
        }

        // POST: bookorders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,custname,total,orderdate")] bookorder bookorder)
        {
            if (id != bookorder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookorder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!bookorderExists(bookorder.Id))
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
            return View(bookorder);
        }

        // GET: bookorders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookorder = await _context.bookorder
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookorder == null)
            {
                return NotFound();
            }

            return View(bookorder);
        }

        // POST: bookorders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookorder = await _context.bookorder.FindAsync(id);
            if (bookorder != null)
            {
                _context.bookorder.Remove(bookorder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool bookorderExists(int id)
        {
            return _context.bookorder.Any(e => e.Id == id);
        }
    }
}

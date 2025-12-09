using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab5.Data;
using lab5.Models;

namespace lab5.Controllers
{
    public class booksController : Controller
    {
        private readonly lab5Context _context;

        public booksController(lab5Context context)
        {
            _context = context;
        }

        // GET: books
        public async Task<IActionResult> Index()
        {
            return View(await _context.book.ToListAsync());
        }

        // GET: books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        public async Task<IActionResult> search()
        {

            List<book> brItems = new List<book>();

            return View(brItems);

        }
        public async Task<IActionResult> searchall()
        {

            {
                book brItem = new book();

                return View(brItem);
            }
        }

        [HttpPost]

        public async Task<IActionResult> SearchAll(string tit)
        {
            var bkItems = await _context.book.FromSqlRaw("select * from book where title = '" + tit + "' ").FirstOrDefaultAsync();

            return View(bkItems);
        }

        
    



        [HttpPost]
        public async Task<IActionResult> Search(string s)
        {
            var bkItems = await _context.book.FromSqlRaw("select * from book where title LIKE '%" + s + "%' ").ToListAsync();
            return View(bkItems);
        }

        // GET: books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,title,info,price,discount,pubdate,category,bookquantity,imgfile")] book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }


        public async Task<IActionResult> orderdetail()
        {

            var orBooks = await _context.orderdetail.FromSqlRaw("select orders.Id as id, name as customer, title as booktitle, orders.quantity as quantity from book, orders, usersaccounts  where book.id = orders.bookid and orders.custid = usersaccounts.id  ").ToListAsync();
            return View(orBooks);


        }


        // GET: books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,title,info,price,discount,pubdate,category,bookquantity,imgfile")] book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!bookExists(book.Id))
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
            return View(book);
        }

        // GET: books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.book.FindAsync(id);
            if (book != null)
            {
                _context.book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool bookExists(int id)
        {
            return _context.book.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyMoneyTrack.Data;
using EasyMoneyTrack.Models;
using System.Security.Claims;

namespace EasyMoneyTrack.Areas.Account.Controllers
{
    [Area("Account")]
    public class SpendingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpendingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Spendings
        public async Task<IActionResult> Index()
        {
           
            
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var applicationDbContext = _context.Spending.Include(s => s.IdentityUser).Where(s => s.UserId == claim.Value);
                return View(await applicationDbContext.ToListAsync());

            }
            else
            {
                return NotFound();
            }



        }

        // GET: Account/Spendings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spending = await _context.Spending
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spending == null)
            {
                return NotFound();
            }

            return View(spending);
        }

        // GET: Account/Spendings/Create
        public IActionResult Create()
        {

            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                Spending model = new Spending()
                {
                    Withdraw = 0,
                    Date = DateTime.Now,
                    UserId = claim.Value
                };
                var currBalance = GetCurrentBlance(claim.Value);
                ViewBag.cblance = currBalance;
                return View(model);
            }
            return NotFound();

        }

        // POST: Account/Spendings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Withdraw,Date,UserId")] Spending spending)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (GetCurrentBlance(claim.Value)>= spending.Withdraw)
                {
                    _context.Add(spending);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.msg= "You don't have enough money ! ";
                    return View(spending);
                }
            }
            return View(spending);
        }

        // GET: Account/Spendings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spending = await _context.Spending.FindAsync(id);
            if (spending == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", spending.UserId);
            return View(spending);
        }

        // POST: Account/Spendings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Withdraw,Date,UserId")] Spending spending)
        {
            if (id != spending.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spending);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpendingExists(spending.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", spending.UserId);
            return View(spending);
        }

        // GET: Account/Spendings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spending = await _context.Spending
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spending == null)
            {
                return NotFound();
            }

            return View(spending);
        }

        // POST: Account/Spendings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spending = await _context.Spending.FindAsync(id);
            _context.Spending.Remove(spending);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpendingExists(int id)
        {
            return _context.Spending.Any(e => e.Id == id);
        }

        public double GetCurrentBlance(string id)
        {
            Double deposit = 0.0, withDraw = 0.0;
            var totalDeposit = _context.Saving.Where(u => u.UserId == id).ToList();
            var toTakWithdraw = _context.Spending.Where(u => u.UserId == id).ToList();
            foreach (var item in totalDeposit)
            {
                deposit += item.Deposit;
            }
            foreach (var item in toTakWithdraw)
            {
                withDraw += item.Withdraw;
            }
            var currentBalance = deposit - withDraw;
            return currentBalance;
        }

    }
}

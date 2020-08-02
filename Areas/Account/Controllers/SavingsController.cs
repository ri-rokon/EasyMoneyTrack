using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyMoneyTrack.Data;
using EasyMoneyTrack.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EasyMoneyTrack.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class SavingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Savings
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var applicationDbContext = _context.Saving.Include(s => s.IdentityUser).Where(s=>s.UserId==claim.Value);
                return View(await applicationDbContext.ToListAsync());

            }
            else
            {
                return NotFound();
            }
            

        }

        // GET: Account/Savings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Saving
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saving == null)
            {
                return NotFound();
            }

            return View(saving);
        }

        // GET: Account/Savings/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Account/Savings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Deposit,Date,UserId")] Saving saving)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saving);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", saving.UserId);
            return View(saving);
        }

        // GET: Account/Savings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Saving.FindAsync(id);
            if (saving == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", saving.UserId);
            return View(saving);
        }

        // POST: Account/Savings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Deposit,Date,UserId")] Saving saving)
        {
            if (id != saving.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saving);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingExists(saving.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", saving.UserId);
            return View(saving);
        }

        // GET: Account/Savings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Saving
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saving == null)
            {
                return NotFound();
            }

            return View(saving);
        }

        // POST: Account/Savings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saving = await _context.Saving.FindAsync(id);
            _context.Saving.Remove(saving);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingExists(int id)
        {
            return _context.Saving.Any(e => e.Id == id);
        }
    }
}

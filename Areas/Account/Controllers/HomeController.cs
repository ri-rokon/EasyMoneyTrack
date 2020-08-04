using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EasyMoneyTrack.Models;
using EasyMoneyTrack.Data;
using System.Security.Claims;
using EasyMoneyTrack.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace EasyMoneyTrack.Controllers
{
    [Area("Account")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var Claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(Claim!=null)
            {
                List<Saving> savingList = _context.Saving.Where(u => u.UserId == Claim.Value).ToList();
                List<Spending> spendingList = _context.Spending.Where(u => u.UserId == Claim.Value).ToList();

                IndexView modelObject = new IndexView()
                {
                    Saving=savingList,
                    Spending=spendingList,
                };

                ViewBag.CurrentBallance = GetCurrentBlance(Claim.Value);

                return View(modelObject);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

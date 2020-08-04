//using EasyMoneyTrack.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace EasyMoneyTrack.ViewModel
//{
    
//    public class CurrentBalance:ClaimsIdentity
//    {
//        private readonly ApplicationDbContext _context;

//        public CurrentBalance( ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public double GetCurrentBlance(string id)
//        {
//            Double deposit = 0.0, withDraw = 0.0;
//            var totalDeposit = _context.Saving.Where(u => u.UserId == id).ToList();
//            var toTakWithdraw= _context.Spending.Where(u => u.UserId == id).ToList();
//            foreach (var item in totalDeposit)
//            {
//                deposit += item.Deposit;
//            }
//            foreach (var item in toTakWithdraw)
//            {
//                withDraw += item.Withdraw;
//            }
//            var currentBalance = deposit - withDraw;
//            return currentBalance;
//        }

//    }
//}

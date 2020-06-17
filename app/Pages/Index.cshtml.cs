using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesContacts.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Prometheus;

namespace RazorPagesContacts.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly Counter TickTock =
            Metrics.CreateCounter("sampleapp_ticks_total", "Just keeps on ticking");

        private readonly AppDbContext _db;
        private static readonly Random random = new Random();
        public readonly int MaxShowCount = 5;
        public bool runTransactionGeneration { get; set; } = true;

        public IndexModel(AppDbContext db, AppConfiguration appConfig)
        {
            _db = db;
            AppConfiguration = appConfig;
        }

        public IList<Transaction> Transactions { get; private set; }

        public AppConfiguration AppConfiguration { get; }

        public async Task OnGetAsync()
        {
            Transactions = await _db.Transactions.AsNoTracking().OrderByDescending(x=>x.Id).Take(MaxShowCount).ToListAsync();
            Task.Run(() => GenerateTransactions());
        }

        private static string _getRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[8];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        private static double _getRandomDouble()
        {
            return ((double) random.Next(0,99999999)) / 100;
        }

        public Task GenerateTransactions()
        {
            while (true) {
                Console.WriteLine("Before task sleep...");
                //await Task.Delay(200);
                while (runTransactionGeneration) {
                    Console.WriteLine("Creating Transaction...");

                    Transaction t = new Transaction{
                        Name = _getRandomString(),
                        Amount = _getRandomDouble()
                    };
                    Console.WriteLine("Adding Transaction...");
                    _db.Transactions.Add(t);
                    Console.WriteLine("Saving Transaction...");
                    _db.SaveChanges();
                    TickTock.Inc();
                    Console.WriteLine("Refreshing Transactions...");
                    //Transactions = await _db.Transactions.AsNoTracking().OrderByDescending(x=>x.Id).Take(MaxShowCount).ToListAsync();
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var transaction = await _db.Transactions.FindAsync(id);

            if (transaction != null)
            {
                _db.Transactions.Remove(transaction);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}

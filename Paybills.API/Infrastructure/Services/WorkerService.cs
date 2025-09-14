using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paybills.API.Data;
using Paybills.API.Infrastructure.Services;

namespace Paybills.API.Services
{
    public class WorkerService
    {
        public bool Stopped { get; set; }
        public int ProcessIntervalInHours { get; set; }

        private const int DEFAULT_PROCESS_INTERVAL = 1;
        private SESService _simpleEmailService;

        public WorkerService(SESService simpleEmailService)
        {
            _simpleEmailService = simpleEmailService;
            ProcessIntervalInHours = DEFAULT_PROCESS_INTERVAL;
        }

        public async void StartWorker()
        {
            Console.WriteLine("Starting email sender service...");
            await ExecuteEmailSending();
            Console.WriteLine("Email sender service started");
        }

        private async Task ExecuteEmailSending()
        {
            while (!Stopped)
            {
                ProcessBills(GetBillsDue());

                await Task.Delay(TimeSpan.FromHours(ProcessIntervalInHours));
            }
        }

        private List<BillsDue> GetBillsDue()
        {
            var billsDue = new List<BillsDue>();
            using (var context = new DataContext())
            {
                var initialDate = DateTime.Now;
                var finalDate = DateTime.Now.AddDays(3);

                var bills = context.Bills
                    .Include(b => b.BillType)
                    .Include(b => b.Users)
                    .Where(b => b.DueDate > initialDate && b.DueDate < finalDate)
                    .AsNoTracking()
                    .ToList();

                foreach (var bill in bills)
                {
                    if (bill.Users.Take(1).FirstOrDefault().EmailValidated)
                    {
                        var userId = bill.Users.Take(1).FirstOrDefault().Id;
                        var userEmail = bill.Users.Take(1).FirstOrDefault().Email;
                        billsDue.Add(new BillsDue(userId, bill.BillType.Description, userEmail, bill.DueDate));
                    }
                }
            }
            return billsDue;
        }

        private void ProcessBills(List<BillsDue> billsDue)
        {
            foreach (var bill in billsDue)
            {
                SendEmailAsync(bill);
            }
        }

        private async void SendEmailAsync(BillsDue billDue)
        {
            var result = await _simpleEmailService.SendEmailAsync(
                new List<string>() { billDue.UserEmail },
                null,
                null,
                $"This is a reminder that you have bills near the due date: {billDue.Description} - {billDue.DueDate:d}",
                "",
                "Billminder - bill(s) near due date",
                "admin@billminder.com.br");
        }
    }

    public class BillsDue
    {
        public int UserId { get; set; }
        public string Description { get; set; }
        public string UserEmail { get; set; }
        public DateTime? DueDate { get; set; }

        public BillsDue(int userId, string description, string userEmail, DateTime? dueDate)
        {
            UserId = userId;
            Description = description;
            DueDate = dueDate;
            UserEmail = userEmail;
        }
    }


}
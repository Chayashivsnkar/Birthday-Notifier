using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BirthdayNotifier.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class BirthdayNotifierService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public BirthdayNotifierService(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;

        var accountSid = _config["Twilio:AccountSid"];
        var authToken = _config["Twilio:AuthToken"];
        TwilioClient.Init(accountSid, authToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(9);
            if (now > targetTime)
                targetTime = targetTime.AddDays(1);

            var wait = targetTime - now;
            await Task.Delay(wait, stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var today = DateTime.Today;

            var users = await db.Users
                .Where(u => u.DOB.HasValue &&
                            u.DOB.Value.Month == today.Month &&
                            u.DOB.Value.Day == today.Day &&
                            !string.IsNullOrEmpty(u.PhoneNumber))
                .ToListAsync();

            var fromNumber = new PhoneNumber(_config["Twilio:PhoneNumber"]);

            foreach (var user in users)
            {
                var msg = $"Hi {user.Name ?? "there"}, Happy Birthday! 🎉";
                await MessageResource.CreateAsync(
                    body: msg,
                    from: fromNumber,
                    to: new PhoneNumber(user.PhoneNumber)
                );
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Wait 24 hrs
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using BirthdayNotifier.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

[ApiController]
[Route("api/[controller]")]
public class BirthdayController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public BirthdayController(ApplicationDbContext db)
    {
        _db = db;

        // Initialize Twilio (you can move this to config/environment in real apps)
        TwilioClient.Init(
            "ACe06299b978294f613ed0f6ab63db7eb2", // Your Twilio SID
            "7e6f4b08eb176cc3c16ac1cee3bdbc55"    // Your Twilio Auth Token
        );
    }

    [HttpPost("trigger")]
    public IActionResult TriggerBirthdayCheck()
    {
        var today = DateTime.Today;

        var users = _db.Users
            .Where(u => u.DOB.HasValue &&
                        u.DOB.Value.Month == today.Month &&
                        u.DOB.Value.Day == today.Day &&
                        !string.IsNullOrEmpty(u.PhoneNumber))
            .ToList();

        foreach (var user in users)
        {
            var message = $"Hi {user.Name ?? "there"}, Happy Birthday! 🎉";
            MessageResource.Create(
                body: message,
                from: new PhoneNumber("+18146793663"),
                to: new PhoneNumber(user.PhoneNumber)
            );
        }

        return Ok(new
        {
            Message = "Birthday check completed",
            Count = users.Count
        });
    }
}

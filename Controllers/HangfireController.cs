using Microsoft.AspNetCore.Mvc;
using Hangfire;

namespace hangfire_api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class HangfireController : ControllerBase
{
    [HttpPost]
    public IActionResult FireAndForgetJob()
    {
        var jobId = BackgroundJob.Enqueue(() => SendWelcomeEmail("Welcome to our app"));

        return Ok($"Job ID : {jobId}. Welcome email send to the user!");
    }

    public void SendWelcomeEmail(string text) => Console.WriteLine(text);

    [HttpPost]
    public IActionResult DelayedJob()
    {
        int timeInSeconds = 30;
        var jobId = BackgroundJob.Schedule(() => SendWelcomeEmail("Welcome to our app"), TimeSpan.FromSeconds(timeInSeconds));

        return Ok($"Job ID : {jobId}. Discount email will be sent in {timeInSeconds}");
    }

    [HttpPost]
    public IActionResult RecurringJobs()
    {
        RecurringJob.AddOrUpdate("recurringJob", () => Console.WriteLine("Database updated"), Cron.Minutely);
        return Ok("Database check job initiaited");
    }

    [HttpPost]
    public IActionResult ContinuousJob()
    {
        int timeInSeconds = 30;
        var parentJobId = BackgroundJob.Schedule(() => Console.WriteLine("You asked to be subscribed"), TimeSpan.FromSeconds(timeInSeconds));

        BackgroundJob.ContinueJobWith(parentJobId, () => Console.WriteLine("You were unsubscribed"));

        return Ok($"Confirmation job created");
    }
}

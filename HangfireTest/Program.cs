using Hangfire;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseHangfireDashboard();

app.MapHangfireDashboard();
app.MapGet("/", ([FromServices] IBackgroundJobClient backgroundJobs) =>
{
    backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
    var enqueueAt = DateTimeOffset.Now.AddDays(1);
    backgroundJobs.Schedule(() => Console.WriteLine("Hello world from Hangfire! " + enqueueAt), enqueueAt);
});

app.Run();

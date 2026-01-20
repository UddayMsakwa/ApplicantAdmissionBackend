using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);


LogManager.Setup().LoadConfigurationFromFile("nlog.config", optional: true);
builder.Logging.ClearProviders();
builder.Logging.AddNLog();


builder.Services.AddDbContext<ApplicantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient("DictionaryApi", client =>
{
    var baseUrl = builder.Configuration["DictionaryApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("DictionaryApi:BaseUrl is missing in Jobs appsettings.json");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var dictMinutes = builder.Configuration.GetValue<int>("Jobs:DictionarySyncIntervalMinutes", 30);
    var notifSeconds = builder.Configuration.GetValue<int>("Jobs:NotificationSenderIntervalSeconds", 30);

    
    var dictJobKey = new JobKey("DictionarySyncJob");
    q.AddJob<DictionarySyncJob>(opts => opts.WithIdentity(dictJobKey));
    q.AddTrigger(opts => opts
        .ForJob(dictJobKey)
        .WithIdentity("DictionarySyncJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(dictMinutes).RepeatForever()));

    
    var notifJobKey = new JobKey("NotificationSenderJob");
    q.AddJob<NotificationSenderJob>(opts => opts.WithIdentity(notifJobKey));
    q.AddTrigger(opts => opts
        .ForJob(notifJobKey)
        .WithIdentity("NotificationSenderJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(notifSeconds).RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
await host.RunAsync();

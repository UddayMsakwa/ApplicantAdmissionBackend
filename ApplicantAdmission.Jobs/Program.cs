using System.Net.Http.Headers;
using System.Text;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.Jobs.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<ApplicantDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddHttpClient("DictionaryApi", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();

    var baseUrl = cfg["DictionaryApi:BaseUrl"];
    var user = cfg["DictionaryApi:Username"];
    var pass = cfg["DictionaryApi:Password"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("DictionaryApi:BaseUrl is missing.");

    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");

    if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass))
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
    }
});


builder.Services.AddQuartz(q =>
{
    
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.ScheduleJob<DictionarySyncJob>(trigger => trigger
        .WithIdentity("DictionarySyncTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(builder.Configuration.GetValue<int>("Jobs:DictionarySyncIntervalMinutes"))
            .RepeatForever()
        )
    );

    q.ScheduleJob<NotificationSenderJob>(trigger => trigger
        .WithIdentity("NotificationSenderTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(builder.Configuration.GetValue<int>("Jobs:NotificationSenderIntervalSeconds"))
            .RepeatForever()
        )
    );
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();

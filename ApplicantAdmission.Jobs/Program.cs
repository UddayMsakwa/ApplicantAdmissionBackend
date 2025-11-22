using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApplicantAdmission.DataAccess;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddNLog();


builder.Services.AddDbContext<ApplicantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Run();

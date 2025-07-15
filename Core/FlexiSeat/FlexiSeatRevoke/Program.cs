using FlexiSeat.DbContext;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
     .ConfigureServices(services =>
     {
         var configuration = new ConfigurationBuilder()
             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();

         var connectionString = configuration.GetConnectionString("MyConnectionString");

         services.AddDbContext<FlexiSeatDbContext>(options =>
             options.UseSqlServer(connectionString));
     })
    .Build();

host.Run();

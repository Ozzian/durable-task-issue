using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

[assembly: FunctionsStartup(typeof(FunctionApp5.Startup))]
namespace FunctionApp5
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddDurableClientFactory( c =>
            {
                c.TaskHub = builder.GetContext().Configuration["AzureFunctionsJobHost:extensions:durableTask:hubName"];
            });
        }
    }
}

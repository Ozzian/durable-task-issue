using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace FunctionApp5
{
    public class Function1
    {
        private readonly IDurableClient _durableClient;

        public Function1(IDurableClientFactory durableClientFactory)
        {
            _durableClient = durableClientFactory.CreateClient();
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var entityId = new EntityId(nameof(MyEntity), "Hullo");
            await _durableClient.SignalEntityAsync<IMyEntity>(new EntityId(nameof(MyEntity), "Hullo"), x => x.SetName("Hullo"));
            var entityRepsonse = await _durableClient.ReadEntityStateAsync<MyEntity>(entityId);
            var entity = entityRepsonse.EntityState;
            return new OkObjectResult($"{entity?.Name} - I am {entity?.Count}");
        }
    }

    public interface IMyEntity
    {
        void SetName(string name);
    }

    public class MyEntity : IMyEntity
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public void SetName(string name)
        {
            Name = name;
            Count++;
        }

        [FunctionName(nameof(MyEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        {
            return ctx.DispatchAsync<MyEntity>();
        }
    }
}

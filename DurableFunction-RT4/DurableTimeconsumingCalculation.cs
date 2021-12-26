using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Timecheck
{
    public class ConsumerParameter {
        public int runs {get;set;}
        public int value { get; set; }
    }
    public static class DurableTimeconsumingCalculation
    {
        [FunctionName("DurableTimeconsumingCalculation")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var outputs = new List<string>();
            ConsumerParameter cParams = context.GetInput<ConsumerParameter>();
            log.LogInformation($"DurableTimeconsumingCalculation statring with runs={cParams.runs} and value={cParams.value}...");

            for(int i=0; i<cParams.runs; i++){
                ConsumerParameter tmpParam = new ConsumerParameter() { runs = i, value = cParams.value };
                outputs.Add(await context.CallActivityAsync<string>("DurableTimeconsumingCalculation_Consumer", tmpParam));
            }
            
            return outputs;
        }

        [FunctionName("DurableTimeconsumingCalculation_Consumer")]
        public static string Consumer([ActivityTrigger] ConsumerParameter tmpParam, ILogger log)
        {
            var res = ConsumeCPU(tmpParam.value);//.GetAwaiter().GetResult();
            log.LogInformation($"DurableTimeconsumingCalculation_Consumer Func has run at round {tmpParam.runs} with result {res} ({System.DateTime.UtcNow.ToLongTimeString()})");
            var msg = $"Consumer finished run {tmpParam.runs} with {res}";
            log.LogInformation(msg);
            return msg;
        }

        public static int ConsumeCPU(int n){
            if(n<=2) return n;
            return ConsumeCPU(n-1) + ConsumeCPU(n-2) - ConsumeCPU(n-3);
        }

        [FunctionName("DurableTimeconsumingCalculation_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"Request URI: {req.RequestUri.ToString()}");
            ConsumerParameter cParams = new ConsumerParameter() { runs = 10, value = 40 };
            var qs = req.RequestUri.ParseQueryString();          
            string runs_param = qs.Get("runs");
            if(runs_param != null) 
                cParams.runs = int.Parse(runs_param);
            string value_param = qs.Get("value");
            if(value_param != null)
                cParams.value = int.Parse(value_param);           
            log.LogInformation($"DurableTimeconsumingCalculation_HttpStart started with runs={cParams.runs} and value={cParams.value}...");

            string instanceId = await starter.StartNewAsync("DurableTimeconsumingCalculation", cParams);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

    }
}
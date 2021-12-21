using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Timecheck
{
    public static class TimeconsumingCalculation
    {
        [FunctionName("TimeconsumingCalculation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string runs_param = req.Query["runs"];
            int runs = 2;
            if(!String.IsNullOrEmpty(runs_param)) 
                runs = int.Parse(runs_param);
            string value_param = req.Query["value"];
            int value = 26;
            if(!String.IsNullOrEmpty(value_param)) 
                value = int.Parse(value_param);
            log.LogInformation($"-- C# HTTP trigger function statring with runs={runs} and value={value} -------------------");

            for(int i=0; i<runs; i++){
                var res = ConsumeCPU(value);//.GetAwaiter().GetResult();
                log.LogInformation($"  default function has run round {i+1} with result {res} ({System.DateTime.UtcNow.ToLongTimeString()})");
            }
            var msg = $"-- finished after {runs} runs! ----------------------------------";
            log.LogInformation(msg);
            return new OkObjectResult(msg);
        }

        public static int ConsumeCPU(int n){
            if(n<=2) return n;
            return ConsumeCPU(n-1) + ConsumeCPU(n-2) - ConsumeCPU(n-3);
        }

    }
}

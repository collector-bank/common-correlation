namespace Collector.Common.Correlation.NetCore.WebApi.Example.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    public class ExampleController : Controller
    {
        [HttpGet]
        [Route("api/example")]
        [TypeFilter(typeof(CorrelationIdActionFilter))]
        public async Task<string> Get()
        {
            return await Task.Run(() => CorrelationState.GetCurrentCorrelationId()?.ToString());
        }
    }
}
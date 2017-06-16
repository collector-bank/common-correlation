// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleController.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Collector.Common.Correlation.Net46.WebApi.Example.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ExampleController : ApiController
    {
        [HttpGet]
        [Route("api/example")]
        public async Task<HttpResponseMessage> Get()
        {
            return await Task.Run(() => Request.CreateResponse(HttpStatusCode.OK, CorrelationState.GetCurrentCorrelationId()));
        }
    }
}
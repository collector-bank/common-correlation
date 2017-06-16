
using Collector.Common.Correlation.Net45.WebApi.Example;

using Microsoft.Owin;

namespace Collector.Common.Correlation.Net45.WebApi.Example
{
    using System.Web.Http;

    using Microsoft.Owin;
    using Microsoft.Owin.Infrastructure;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            SignatureConversions.AddConversions(appBuilder);
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }
    }
}
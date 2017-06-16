namespace Collector.Common.Correlation.Net46.WebApi.Example
{
    using System.Web.Http;

    using Microsoft.Owin.Infrastructure;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }
    }
}
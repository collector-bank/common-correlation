// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Collector.Common.Correlation.AspNet.Samples
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Http;
    
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            HttpContextCorrelationState.Enable();
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Collector.Common.Correlation.AspNet.Samples.Controllers
{
    using System.Web.Mvc;
    
    using Collector.Common.Correlation.AspNet.Samples.Filters;

    public class HomeController : Controller
    {
        [CorrelationIdActionFilter(true)]
        public ActionResult Index()
        {
            return View(CorrelationState.GetCurrentCorrelationId());
        }
    }
}
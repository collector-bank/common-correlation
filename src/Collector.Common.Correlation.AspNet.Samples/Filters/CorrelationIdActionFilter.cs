// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationIdActionFilter.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Collector.Common.Correlation.AspNet.Samples.Filters
{
    using System;
    using System.Web.Mvc;

    public class CorrelationIdActionFilter : ActionFilterAttribute
    {
        private readonly bool _setCorrelationIdFromContext;

        public CorrelationIdActionFilter(bool setCorrelationIdFromContext = false)
        {
            _setCorrelationIdFromContext = setCorrelationIdFromContext;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Guid? existingCorrelationId = null;

            if (_setCorrelationIdFromContext)
            {
                try
                {
                    existingCorrelationId = Guid.Parse(filterContext.HttpContext.Request.QueryString["Context"]);
                }
                catch
                {
                }
            }

            CorrelationState.InitializeCorrelation(existingCorrelationId);
        }
    }
}
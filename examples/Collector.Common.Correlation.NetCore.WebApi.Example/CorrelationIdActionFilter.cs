// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationIdActionFilter.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Collector.Common.Correlation.NetCore.WebApi.Example
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class CorrelationIdActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            CorrelationState.InitializeCorrelation();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            CorrelationState.ClearCorrelation();
            base.OnActionExecuted(context);
        }
    }
}
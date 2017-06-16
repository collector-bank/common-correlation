namespace Collector.Common.Correlation.Net45.WebApi.Example
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class CorrelationIdActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            CorrelationState.InitializeCorrelation();
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            CorrelationState.ClearCorrelation();

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}
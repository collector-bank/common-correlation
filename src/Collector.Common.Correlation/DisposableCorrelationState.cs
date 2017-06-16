namespace Collector.Common.Correlation
{
    using System;
    
    /// <summary>
    /// Disposing this class clears the correlation.
    /// </summary>
    public class DisposableCorrelationState : IDisposable
    {
        /// <summary>
        /// The Correlation id for the current scope.
        /// </summary>
        public Guid CorrelationId { get; }

        internal DisposableCorrelationState(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Clears the current correlation.
        /// </summary>
        public void Dispose()
        {
            CorrelationState.ClearCorrelation();
        }

        public static implicit operator Guid(DisposableCorrelationState disposableCorrelationState)
        {
            return disposableCorrelationState.CorrelationId;
        }
    }
}
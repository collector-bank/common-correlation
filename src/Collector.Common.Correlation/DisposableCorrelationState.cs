namespace Collector.Common.Correlation
{
    using System;
    
    /// <summary>
    /// Disposing this class clears the correlation.
    /// </summary>
    public class DisposableCorrelationState : IDisposable
    {
        private readonly bool _clearOnDispose;

        /// <summary>
        /// The Correlation id for the current scope.
        /// </summary>
        public Guid CorrelationId { get; }

        internal DisposableCorrelationState(Guid correlationId, bool clearOnDispose = true)
        {
            _clearOnDispose = clearOnDispose;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Clears the current correlation.
        /// </summary>
        public void Dispose()
        {
            if (_clearOnDispose)
                CorrelationState.ClearCorrelation();
        }

        public static implicit operator Guid(DisposableCorrelationState disposableCorrelationState)
        {
            return disposableCorrelationState.CorrelationId;
        }
    }
}
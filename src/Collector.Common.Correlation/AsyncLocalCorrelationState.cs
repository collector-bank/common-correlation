#if NET45
#else
namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    internal class AsyncLocalCorrelationState : ICorrelationState
    {
        private static readonly AsyncLocal<Guid?> CorrelationId = new AsyncLocal<Guid?>();
        private static readonly AsyncLocal<ConcurrentDictionary<string, object>> CorrelationValues = new AsyncLocal<ConcurrentDictionary<string, object>>();

        public int Priority => 100;

        public void InitializeCorrelation(Guid newCorrelationId)
        {
            CorrelationId.Value = newCorrelationId;
            CorrelationValues.Value = new ConcurrentDictionary<string, object>();
        }

        public void ClearCorrelation()
        {
            CorrelationId.Value = null;
            CorrelationValues.Value = null;
        }

        public Guid? GetCurrentCorrelationId() => CorrelationId.Value;

        public bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            CorrelationValues.Value?.AddOrUpdate(name, value, (s, o) => value);

            return CorrelationValues.Value != null;
        }

        public IEnumerable<KeyValuePair<string, object>> GetCorrelationValues() => CorrelationValues.Value;

        public object GetValue(string name) =>
            CorrelationValues.Value != null && CorrelationValues.Value.TryGetValue(name, out var value) ? value : null;

        public T GetValue<T>(string name)
            where T : class =>
            GetValue(name) as T;

    }
}
#endif
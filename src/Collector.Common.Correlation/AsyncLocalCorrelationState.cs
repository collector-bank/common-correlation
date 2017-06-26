#if NET45
#else
namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    internal static class AsyncLocalCorrelationState
    {
        private static readonly AsyncLocal<Guid?> CorrelationId = new AsyncLocal<Guid?>();
        private static readonly AsyncLocal<ConcurrentDictionary<string, object>> CorrelationValues = new AsyncLocal<ConcurrentDictionary<string, object>>();

        public static void InitializeCorrelation(Guid newCorrelationId)
        {
            CorrelationId.Value = newCorrelationId;
            CorrelationValues.Value = new ConcurrentDictionary<string, object>();
        }

        public static void ClearCorrelation()
        {
            CorrelationId.Value = null;
            CorrelationValues.Value = null;
        }

        public static Guid? GetCurrentCorrelationId() => CorrelationId.Value;

        public static bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            CorrelationValues.Value?.AddOrUpdate(name, value, (s, o) => value);

            return CorrelationValues.Value != null;
        }

        public static IEnumerable<KeyValuePair<string, object>> GetCorrelationValues() => CorrelationValues.Value;
    }
}
#endif
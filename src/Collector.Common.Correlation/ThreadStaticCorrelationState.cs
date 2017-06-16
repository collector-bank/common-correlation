
namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    
    internal static class ThreadStaticCorrelationState
    {
        [ThreadStatic]
        private static Guid? correlationId;

        [ThreadStatic]
        private static int? threadId;

        [ThreadStatic]
        private static ConcurrentDictionary<string, object> correlationDictionary;
        
        public static void InitializeCorrelation(Guid newCorrelationId)
        {
            correlationId = newCorrelationId;
            threadId = Thread.CurrentThread.ManagedThreadId;
            correlationDictionary = new ConcurrentDictionary<string, object>();
        }
        
        public static void ClearCorrelation()
        {
            threadId = null;
            correlationId = null;
            correlationDictionary?.Clear();
            correlationDictionary = null;
        }
        
        public static Guid? GetCurrentCorrelationId()
        {
            if (HasActiveCorrelationSession())
                return correlationId;

            return null;
        }
        
        public static bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            if (!HasActiveCorrelationSession())
                return false;

            if (correlationDictionary == null)
                correlationDictionary = new ConcurrentDictionary<string, object>();

            correlationDictionary.AddOrUpdate(name, value, (s, o) => value);

            return true;
        }
        
        public static IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            return correlationDictionary;
        }

        private static bool HasActiveCorrelationSession()
        {
            if (threadId == null)
                return false;
            if (correlationId == null)
                return false;
            return threadId.Value == Thread.CurrentThread.ManagedThreadId;
        }
    }
}
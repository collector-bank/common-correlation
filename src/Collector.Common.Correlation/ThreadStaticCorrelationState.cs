namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    
    internal class ThreadStaticCorrelationState : ICorrelationState
    {
        [ThreadStatic]
        private static Guid? correlationId;

        [ThreadStatic]
        private static int? threadId;

        [ThreadStatic]
        private static ConcurrentDictionary<string, object> correlationDictionary;

        public int Priority => 200;

        public void InitializeCorrelation(Guid newCorrelationId)
        {
            correlationId = newCorrelationId;
            threadId = Thread.CurrentThread.ManagedThreadId;
            correlationDictionary = new ConcurrentDictionary<string, object>();
        }
        
        public void ClearCorrelation()
        {
            threadId = null;
            correlationId = null;
            correlationDictionary?.Clear();
            correlationDictionary = null;
        }
        
        public Guid? GetCurrentCorrelationId()
        {
            if (HasActiveCorrelationSession())
                return correlationId;

            return null;
        }
        
        public bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            if (!HasActiveCorrelationSession())
                return false;

            if (correlationDictionary == null)
                correlationDictionary = new ConcurrentDictionary<string, object>();

            correlationDictionary.AddOrUpdate(name, value, (s, o) => value);

            return true;
        }
        
        public IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            return correlationDictionary;
        }

        public object GetValue(string name) =>
            correlationDictionary.TryGetValue(name, out var value) ? value : null;

        public T GetValue<T>(string name)
            where T : class =>
            GetValue(name) as T;

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
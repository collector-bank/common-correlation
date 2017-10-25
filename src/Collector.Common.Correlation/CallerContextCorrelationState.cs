#if NET45

namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;
    
    internal class CallerContextCorrelationState : ICorrelationState
    {
        private const string CORRELATION_VALUES = "CorrelationValues";
        private const string CORRELATION_ID = "CorrelationId";

        public int Priority => 100;

        public void InitializeCorrelation(Guid newCorrelationId)
        {
            CallContext.LogicalSetData(CORRELATION_ID, newCorrelationId);
            CallContext.LogicalSetData(CORRELATION_VALUES, new ConcurrentDictionary<string, object>());
        }
        
        public void ClearCorrelation()
        {
            CallContext.FreeNamedDataSlot(CORRELATION_ID);
            CallContext.FreeNamedDataSlot(CORRELATION_VALUES);
        }
        
        public Guid? GetCurrentCorrelationId()
        {
            return CallContext.LogicalGetData(CORRELATION_ID) as Guid?;
        }
        
        public bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            if (!HasActiveCorrelationSession())
            {
                return false;
            }

            var correlationDictionary = (ConcurrentDictionary<string, object>)CallContext.LogicalGetData(CORRELATION_VALUES);

            correlationDictionary.AddOrUpdate(name, value, (s, o) => value);

            return true;
        }
        
        public IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            return CallContext.LogicalGetData(CORRELATION_VALUES) as ConcurrentDictionary<string, object>;
        }

        private static bool HasActiveCorrelationSession()
        {
            return CallContext.LogicalGetData(CORRELATION_ID) != null;
        }
    }
}
#endif
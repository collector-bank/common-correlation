// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextCorrelationState.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Collector.Common.Correlation.AspNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class HttpContextCorrelationState : ICorrelationState
    {
        private const string CorrelationIdKey = "collector.CorrelationId";
        private const string CorrelationValueDictionaryKey = "collector.CorrelationValues";

        public int Priority => 10;

        public void InitializeCorrelation(Guid newCorrelationId)
        {
            var httpContext = HttpContext.Current;
            
            if(httpContext == null)
                return;
                
            httpContext.Items[CorrelationIdKey] = newCorrelationId;
            httpContext.Items[CorrelationValueDictionaryKey] = new ConcurrentDictionary<string, object>();
        }

        public void ClearCorrelation()
        {
            var httpContext = HttpContext.Current;
            
            if(httpContext == null)
                return;
            
            if (httpContext.Items.Contains(CorrelationIdKey))
                httpContext.Items.Remove(CorrelationIdKey);
        }

        public Guid? GetCurrentCorrelationId()
        {
            return HttpContext.Current?.Items[CorrelationIdKey] as Guid?;
        }

        public bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            var httpContext = HttpContext.Current;
            
            if(httpContext == null)
                return false;

            var correlationValuesDictionary = httpContext.Items[CorrelationValueDictionaryKey] as ConcurrentDictionary<string, object>;

            if (correlationValuesDictionary == null)
            {
                correlationValuesDictionary = new ConcurrentDictionary<string, object>();

                httpContext.Items[CorrelationValueDictionaryKey] = correlationValuesDictionary;
            }

            correlationValuesDictionary[name] = value;

            return true;
        }

        public IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            var httpContext = HttpContext.Current;
            
            if(httpContext == null)
                return Enumerable.Empty<KeyValuePair<string, object>>();

            return httpContext.Items[CorrelationValueDictionaryKey] as ConcurrentDictionary<string, object> ?? new ConcurrentDictionary<string, object>();
        }

        public object GetValue(string name)
            => HttpContext.Current.Items.Contains(name) ? HttpContext.Current.Items[name] : null;

        public T GetValue<T>(string name)
            where T : class
            => GetValue(name) as T;

        public static void Enable()
        {
            CorrelationState.Use(new HttpContextCorrelationState());
        }
    }
}
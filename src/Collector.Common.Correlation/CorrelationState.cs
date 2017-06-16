// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationState.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Tracks and keeps the current correlation state
    /// </summary>
    public static class CorrelationState
    {
        [ThreadStatic]
        private static Guid? correlationId;

        [ThreadStatic]
        private static int? threadId;

        [ThreadStatic]
        private static ConcurrentDictionary<string, object> correlationDictionary;

        /// <summary>
        /// Initialize a new correlation state. This is tracked by thread id.
        /// </summary>
        /// <param name="existingCorrelationId">Use this parameter to set the correlation id</param>
        /// <returns>The current correlation id</returns>
        public static Guid InitializeCorrelation(Guid? existingCorrelationId = null)
        {
            threadId = Thread.CurrentThread.ManagedThreadId;
            correlationId = existingCorrelationId ?? Guid.NewGuid();
            correlationDictionary = new ConcurrentDictionary<string, object>();

            return correlationId.Value;
        }

        /// <summary>
        /// Clears the correlation state for this thread
        /// </summary>
        public static void ClearCorrelation()
        {
            threadId = null;
            correlationId = null;
            correlationDictionary?.Clear();
            correlationDictionary = null;
        }

        /// <summary>
        /// Get the current correlation state id
        /// </summary>
        /// <returns>The current correlation id, otherwise null</returns>
        public static Guid? GetCurrentCorrelationId()
        {
            if (HasActiveCorrelationSession())
                return correlationId;

            return null;
        }

        /// <summary>
        /// Adds or updates a correlation value for the current correlation session. Returns false if no current session exists.
        /// </summary>
        /// <param name="name">The name of the value. If the name already exists then the value will be updated.</param>
        /// <param name="value">The correlation value.</param>
        /// <returns>True value was added to the correlation session.</returns>
        public static bool TryAddOrUpdateCorrelationValue(string name, object value)
        {
            if (!HasActiveCorrelationSession())
                return false;

            if (correlationDictionary == null)
                correlationDictionary = new ConcurrentDictionary<string, object>();

            correlationDictionary.AddOrUpdate(name, value, (s, o) => value);

            return true;
        }

        /// <summary>
        /// Gets the correlation key/value pairs for the current correlation session.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            return correlationDictionary ?? Enumerable.Empty<KeyValuePair<string, object>>();
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
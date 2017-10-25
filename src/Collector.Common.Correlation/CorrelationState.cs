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

    /// <summary>
    /// Tracks and keeps the current correlation state
    /// </summary>
    public static class CorrelationState
    {
        private static readonly ConcurrentBag<ICorrelationState> CorrelationStates = new ConcurrentBag<ICorrelationState>();

        static CorrelationState()
        {
            Use(new ThreadStaticCorrelationState());
#if NET45
            Use(new CallerContextCorrelationState());
#else
            Use(new AsyncLocalCorrelationState());
#endif
        }

        public static void Use(ICorrelationState state)
        {
            CorrelationStates.Add(state);
        }

        /// <summary>
        /// Initialize a new correlation state.
        /// </summary>
        /// <param name="existingCorrelationId">Use this parameter to set the correlation id</param>
        /// <returns>
        /// The current correlation id, wraped in a disposable class. 
        /// To clear the correlation, either dipose this object or call the CorrelationState.ClearCorrelation() method.
        /// </returns>
        public static DisposableCorrelationState InitializeCorrelation(Guid? existingCorrelationId = null)
        {
            var correlationId = existingCorrelationId ?? Guid.NewGuid();
            CorrelationStates.OrderBy(x => x.Priority).ForEach(a => a.InitializeCorrelation(correlationId));

            return new DisposableCorrelationState(correlationId);
        }

        /// <summary>
        /// Ensures that a correlation exists by either giving you the existing correlation, or by creating a new.
        /// </summary>
        /// <returns>
        /// If there already existed a correlation, then the disposable returned will not clear the ongoing correlation.
        /// </returns>
        public static DisposableCorrelationState EnsureCorrelation()
        {
            var correlationId = GetCurrentCorrelationId();
            if (correlationId.HasValue)
                return new DisposableCorrelationState(correlationId.Value, clearOnDispose: false);

            return InitializeCorrelation();
        }

        /// <summary>
        /// Clears the current correlation.
        /// </summary>
        public static void ClearCorrelation()
        {
            CorrelationStates.OrderBy(x => x.Priority).ForEach(a => a.ClearCorrelation());
        }

        /// <summary>
        /// Get the current correlation state id
        /// </summary>
        /// <returns>The current correlation id, otherwise null</returns>
        public static Guid? GetCurrentCorrelationId()
        {
            foreach (var correlationState in CorrelationStates.OrderBy(x => x.Priority))
            {
                var correlationId = correlationState.GetCurrentCorrelationId();
                if (correlationId.HasValue)
                    return correlationId;
            }

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
            return CorrelationStates.OrderBy(x => x.Priority).Select(a => a.TryAddOrUpdateCorrelationValue(name, value))
                                                  .ToArray()
                                                  .Any(b => b);
        }

        /// <summary>
        /// Gets the correlation key/value pairs for the current correlation session.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            foreach (var correlationState in CorrelationStates.OrderBy(x => x.Priority))
            {
                var correlationValues = correlationState.GetCorrelationValues();
                if (correlationValues != null)
                    return correlationValues;
            }

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// If you jump across several different threads using different threading methods (as in WPF threading and async await nested) then you can manually resyncronize the correlation state.
        /// </summary>
        /// <returns>True if the syncronization was successful.</returns>
        public static bool TryManuallySyncronizeCorrelationState()
        {
            var correlationId = GetCurrentCorrelationId();

            if (correlationId.HasValue)
            {
                var correlationValues = GetCorrelationValues();
                InitializeCorrelation(correlationId);
                correlationValues?.ForEach(kvp => TryAddOrUpdateCorrelationValue(kvp.Key, kvp.Value));
            }

            return correlationId.HasValue;
        }
    }
}
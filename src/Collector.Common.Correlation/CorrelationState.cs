// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationState.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Tracks and keeps the current correlation state
    /// </summary>
    public static class CorrelationState
    {
        private static readonly IList<Action<Guid>> InitializeCorrelation_ = new List<Action<Guid>>
                                                                             {
#if NET45
                                                                                 CallerContextCorrelationState.InitializeCorrelation,
#endif
                                                                                 ThreadStaticCorrelationState.InitializeCorrelation,
                                                                             };

        private static readonly IList<Action> ClearCorrelation_ = new List<Action>
                                                                  {
#if NET45
                                                                      CallerContextCorrelationState.ClearCorrelation,
#endif
                                                                      ThreadStaticCorrelationState.ClearCorrelation,
                                                                  };

        private static readonly IList<Func<Guid?>> GetCurrentCorrelationId_ = new List<Func<Guid?>>
                                                                              {
#if NET45
                                                                                  CallerContextCorrelationState.GetCurrentCorrelationId,
#endif
                                                                                  ThreadStaticCorrelationState.GetCurrentCorrelationId,
                                                                              };

        private static readonly IList<Func<string, object, bool>> TryAddOrUpdateCorrelationValue_ = new List<Func<string, object, bool>>
                                                                                                    {
#if NET45
                                                                                                        CallerContextCorrelationState.TryAddOrUpdateCorrelationValue,
#endif
                                                                                                        ThreadStaticCorrelationState.TryAddOrUpdateCorrelationValue,
                                                                                                    };

        private static readonly IList<Func<IEnumerable<KeyValuePair<string, object>>>> GetCorrelationValues_ = new List<Func<IEnumerable<KeyValuePair<string, object>>>>
                                                                                                               {
#if NET45
                                                                                                                   CallerContextCorrelationState.GetCorrelationValues,
#endif
                                                                                                                   ThreadStaticCorrelationState.GetCorrelationValues
                                                                                                               };

        /// <summary>
        /// Initialize a new correlation state. This is tracked by thread id.
        /// </summary>
        /// <param name="existingCorrelationId">Use this parameter to set the correlation id</param>
        /// <returns>The current correlation id</returns>
        public static Guid InitializeCorrelation(Guid? existingCorrelationId = null)
        {
            var correlationId = existingCorrelationId ?? Guid.NewGuid();
            InitializeCorrelation_.ForEach(a => a(correlationId));

            return correlationId;
        }

        /// <summary>
        /// Clears the correlation state for this thread
        /// </summary>
        public static void ClearCorrelation()
        {
            ClearCorrelation_.ForEach(a => a());
        }

        /// <summary>
        /// Get the current correlation state id
        /// </summary>
        /// <returns>The current correlation id, otherwise null</returns>
        public static Guid? GetCurrentCorrelationId()
        {
            foreach (var func in GetCurrentCorrelationId_)
            {
                var correlationId = func();
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
            return TryAddOrUpdateCorrelationValue_.Select(a => a(name, value))
                                                  .ToArray()
                                                  .Any(b => b);
        }

        /// <summary>
        /// Gets the correlation key/value pairs for the current correlation session.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, object>> GetCorrelationValues()
        {
            foreach (var func in GetCorrelationValues_)
            {
                var correlationId = func();
                if (correlationId != null)
                    return correlationId;
            }

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICorrelationState.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Collector.Common.Correlation
{
    using System;
    using System.Collections.Generic;

    public interface ICorrelationState
    {
        int Priority { get; }
        
        void InitializeCorrelation(Guid newCorrelationId);

        void ClearCorrelation();

        Guid? GetCurrentCorrelationId();

        bool TryAddOrUpdateCorrelationValue(string name, object value);

        IEnumerable<KeyValuePair<string, object>> GetCorrelationValues();
    }
}
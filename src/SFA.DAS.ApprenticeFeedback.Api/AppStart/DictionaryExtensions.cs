﻿using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Api.AppStart
{
    public static class DictionaryExtensions
    {
        public static void AddIfNotPresent<TKey, TValue>(this IDictionary<TKey, TValue> headers, TKey key, TValue values) where TKey : notnull
        {
            if (headers.ContainsKey(key)) { return; }
            headers.Add(key, values);
        }
    }
}

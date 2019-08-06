﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JazSharp.SpyLogic
{
    internal class SpyInfo
    {
        private static readonly List<SpyInfo> _spies = new List<SpyInfo>();

        internal MethodInfo Method { get; }
        internal Dictionary<object, bool> CallThroughMapping { get; } = new Dictionary<object, bool>();
        internal Dictionary<object, object> ReturnValueMapping { get; } = new Dictionary<object, object>();
        internal Dictionary<object, Exception> ThrowMapping { get; } = new Dictionary<object, Exception>();
        internal Dictionary<object, Queue<object>> ReturnValuesMapping { get; } = new Dictionary<object, Queue<object>>();
        internal Dictionary<object, List<object[]>> CallsLog { get; } = new Dictionary<object, List<object[]>>();

        internal SpyInfo(MethodInfo method)
        {
            Method = method.GetBaseDefinition();
        }

        internal void StopSpying(object key)
        {
            if (CallThroughMapping.ContainsKey(key))
            {
                CallThroughMapping.Remove(key);
            }

            if (ReturnValueMapping.ContainsKey(key))
            {
                ReturnValueMapping.Remove(key);
            }

            if (ThrowMapping.ContainsKey(key))
            {
                ThrowMapping.Remove(key);
            }

            if (ReturnValuesMapping.ContainsKey(key))
            {
                ReturnValuesMapping.Remove(key);
            }

            if (CallsLog.ContainsKey(key))
            {
                CallsLog.Remove(key);
            }
        }

        internal static SpyInfo Create(MethodInfo method)
        {
            var spy = new SpyInfo(method);
            _spies.Add(spy);
            return spy;
        }

        internal static SpyInfo Get(MethodInfo method)
        {
            method = method.GetBaseDefinition();
            return _spies.FirstOrDefault(x => x.Method == method);
        }

        internal static SpyInfo Get(string methodFullName)
        {
            var unpacked = Regex.Match(methodFullName, @"([a-z0-9_]+\.)*([a-z0-9_]+) ([a-z0-9_.\/]+)::([a-z0-9_(),.]+)", RegexOptions.IgnoreCase);
            var classFullName = unpacked.Groups[3].Value.Replace('/', '+');
            var methodAsString = unpacked.Groups[2] + " " + unpacked.Groups[4];

            return _spies.FirstOrDefault(x => x.Method.ToString() == methodAsString && x.Method.DeclaringType.FullName == classFullName);
        }

        internal static void Clear()
        {
            _spies.Clear();
        }
    }
}

// <copyright file="src/Zentient.Analyzers/Engine/BuildEngine.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Engine
{
    public sealed class BuildEngine : IBuildEngine
    {
        private readonly IRegistry _registry;

        /// <inheritdoc/>
        public event Action<ISourceUnit>? BeforeEmit;

        /// <inheritdoc/>
        public event Action<ISourceUnit>? AfterEmit;

        public BuildEngine(IRegistry registry) => _registry = registry;

        /// <inheritdoc/>
        public IReadOnlyList<ISourceUnit> Build(IEnumerable<string> seeds)
        {
            var sorted = TopologicalSort(seeds);
            var emitted = new List<ISourceUnit>();

            foreach (var key in sorted)
            {
                var instr = _registry.GetInstructions(key);
                var unit = instr switch
                {
                    IStubInstructions s => s.Emit(),
                    ITemplateInstructions t => t.Emit(emitted),
                    _ => throw new InvalidOperationException("Unknown instruction type")
                };
                BeforeEmit?.Invoke(unit);
                emitted.Add(unit);
                AfterEmit?.Invoke(unit);
            }

            return emitted;
        }

        private bool TryGetStub(string key, out IStubInstructions stub)
            => (stub = _registry.GetStub(key)) != null;

        private bool TryGetTemplate(string key, out ITemplateInstructions tpl)
            => (tpl = _registry.GetTemplate(key)) != null;


        private List<string> TopologicalSort(IEnumerable<string> seeds)
        {
            var graph = new Dictionary<string, HashSet<string>>();
            void Add(string k)
            {
                if (graph.ContainsKey(k))
                    return;
                var deps = new HashSet<string>();
                if (_registry.Keys.Contains(k) && TryGetStub(k, out var s))
                    foreach (var r in s.Requires)
                        deps.Add(r);
                if (_registry.Keys.Contains(k) && TryGetTemplate(k, out var t))
                    foreach (var r in t.Requires)
                        deps.Add(r);
                graph[k] = deps;
                foreach (var d in deps)
                    Add(d);
            }

            foreach (var k in seeds)
                Add(k);

            var inDeg = graph.ToDictionary(kv => kv.Key, kv => 0);
            foreach (var deps in graph.Values)
                foreach (var d in deps)
                    inDeg[d]++;

            var queue = new Queue<string>(inDeg.Where(kv => kv.Value == 0).Select(kv => kv.Key));
            var result = new List<string>();

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                result.Add(n);
                foreach (var m in graph[n].ToList())
                {
                    graph[n].Remove(m);
                    if (--inDeg[m] == 0)
                        queue.Enqueue(m);
                }
            }

            if (graph.Any(kv => kv.Value.Count > 0))
                throw new InvalidOperationException("Cycle detected in registry graph");

            return result.Where(s => seeds.Contains(s)).ToList();
        }
    }
}

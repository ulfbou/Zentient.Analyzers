// <copyright file="src/Zentient.Analyzers.Core/Engine/BuildEngine.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;
using Zentient.Analyzers.Registry;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Zentient.Analyzers.Engine
{
    /// <summary>
    /// Implements <see cref="IBuildEngine"/> to generate source units from instruction keys.
    /// </summary>
    public sealed class BuildEngine : IBuildEngine
    {
        private readonly IRegistry _registry;
        private readonly Dictionary<string, ISourceUnit> _emittedUnits = new();

        public event Action<ISourceUnit>? BeforeEmit;
        public event Action<ISourceUnit>? AfterEmit;

        public BuildEngine(IRegistry registry)
        {
            _registry = registry;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISourceUnit> Build(IEnumerable<string> instructionKeys)
        {
            // Use HashSet for fast lookup and avoid duplicates
            HashSet<string> requestedKeys = instructionKeys is HashSet<string> set ? set : new HashSet<string>(instructionKeys);
            var sortedKeys = TopologicalSort(requestedKeys);

            var results = new List<ISourceUnit>(sortedKeys.Count);

            foreach (string key in sortedKeys)
            {
                if (!_emittedUnits.TryGetValue(key, out ISourceUnit sourceUnit))
                {
                    sourceUnit = EmitInstructions(_registry.GetInstructions(key));
                    _emittedUnits[key] = sourceUnit;
                }
                results.Add(sourceUnit);
            }

            return results.ToImmutableArray();
        }

        private ISourceUnit EmitInstructions(ICodeInstructions instructions)
        {
            // Only invoke BeforeEmit if a valid source unit is available
            BeforeEmit?.Invoke(null!); // TODO: Replace null with real SourceUnit if available

            ISourceUnit sourceUnit = instructions switch
            {
                IStubInstructions stub => stub.Emit(),
                ITemplateInstructions template => template.Emit(GetEmittedStubs(template)),
                _ => throw new InvalidOperationException($"Unsupported instruction type: {instructions.GetType().Name}")
            };

            AfterEmit?.Invoke(sourceUnit);
            return sourceUnit;
        }

        private IReadOnlyList<ISourceUnit> GetEmittedStubs(ITemplateInstructions instructions)
        {
            // Use capacity for performance if possible
            var stubs = new List<ISourceUnit>(instructions.Requires.Count);
            foreach (string key in instructions.Requires)
            {
                if (_registry.GetInstructions(key) is IStubInstructions)
                {
                    if (_emittedUnits.TryGetValue(key, out ISourceUnit stubUnit))
                    {
                        stubs.Add(stubUnit);
                    }
                    else
                    {
                        // Should not occur if TopologicalSort is correct
                        throw new InvalidOperationException($"Required stub '{key}' was not emitted.");
                    }
                }
            }
            return stubs;
        }

        private List<string> TopologicalSort(HashSet<string> requestedKeys)
        {
            var visited = new HashSet<string>();
            var sortedList = new List<string>(requestedKeys.Count);

            foreach (string key in requestedKeys)
            {
                if (!visited.Contains(key))
                {
                    SortVisit(key, visited, sortedList);
                }
            }

            // Reverse for correct topological order
            sortedList.Reverse();
            return sortedList;
        }

        private void SortVisit(string key, HashSet<string> visited, List<string> sortedList)
        {
            if (visited.Contains(key))
                return;
            visited.Add(key);

            var instructions = _registry.GetInstructions(key);
            foreach (string requiredKey in instructions.Requires)
            {
                SortVisit(requiredKey, visited, sortedList);
            }
            sortedList.Add(key);
        }
    }
}

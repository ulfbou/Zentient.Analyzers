// <copyright file="src/Zentient.Analyzers/Registry/Registry.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry
{
    internal sealed class Registry : IRegistry
    {
        private readonly ImmutableDictionary<string, IStubInstructions> _stubs;
        private readonly ImmutableDictionary<string, ITemplateInstructions> _templates;

        public Registry(ImmutableArray<ICodeInstructions> instrs)
        {
            _stubs = instrs.OfType<IStubInstructions>().ToImmutableDictionary(x => x.Key);
            _templates = instrs.OfType<ITemplateInstructions>().ToImmutableDictionary(x => x.Key);
            Keys = _stubs.Keys.Concat(_templates.Keys).ToImmutableArray();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> Keys { get; }

        /// <inheritdoc/>
        public ICodeInstructions GetInstructions(string key)
            => _stubs.TryGetValue(key, out var s)
               ? s : _templates.TryGetValue(key, out var t)
               ? t : throw new KeyNotFoundException(key);

        /// <inheritdoc/>
        public IStubInstructions GetStub(string key)
            => _stubs.TryGetValue(key, out var s) ? s : throw new KeyNotFoundException(key);

        /// <inheritdoc/>
        public ITemplateInstructions GetTemplate(string key)
            => _templates.TryGetValue(key, out var t) ? t : throw new KeyNotFoundException(key);
    }
}

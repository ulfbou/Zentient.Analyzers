// <copyright file="Results.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// This library contains the canonical source code stubs for Zentient.Abstractions.
// It is intended to be used as a dependency for the Zentient.Analyzers.Templates
// project to ensure all tests use a consistent, compilable baseline.

namespace Zentient.Analyzers.Stubs
{
    /// <summary>
    /// Contains concrete, compliant implementations of the Results abstractions.
    /// </summary>
    public static class Results
    {
        public static string GetCompliantImplementations(string ns, string baseClassName)
            => $@"namespace {ns}.Results
{{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Zentient.Abstractions;
    using Zentient.Abstractions.Results;
    using Zentient.Abstractions.Errors;
    using Zentient.Abstractions.Errors.Definitions;
    using Zentient.Abstractions.Contexts;
    using Zentient.Abstractions.Contexts.Definitions;
    using Zentient.Analyzers.Stubs.Common;

    public sealed class {baseClassName} : IResult
    {{
        public IReadOnlyList<string> Messages {{ get; }}
        public IEnumerable<IErrorInfo<IErrorDefinition>> Errors {{ get; }}

#if NETSTANDARD2_0
        public bool IsSuccess => !Errors.Any();
        public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
#endif
        public {baseClassName}(IEnumerable<string>? messages = null,
                              IEnumerable<IErrorInfo<IErrorDefinition>>? errors = null)
        {{
            Messages = (messages ?? Array.Empty<string>()).AsListReadOnly();
            Errors = errors ?? Enumerable.Empty<IErrorInfo<IErrorDefinition>>();
        }}
    }}

    public sealed class {baseClassName}OfT<TValue> : IResult<TValue>
    {{
        public TValue Value {{ get; }}
        public IReadOnlyList<string> Messages {{ get; }}
        public IEnumerable<IErrorInfo<IErrorDefinition>> Errors {{ get; }}

#if NETSTANDARD2_0
        public bool IsSuccess => !Errors.Any();
        public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
#endif
        public {baseClassName}OfT(TValue value,
                               IEnumerable<string>? messages = null,
                               IEnumerable<IErrorInfo<IErrorDefinition>>? errors = null)
        {{
            Value = value;
            Messages = (messages ?? Array.Empty<string>()).AsListReadOnly();
            Errors = errors ?? Enumerable.Empty<IErrorInfo<IErrorDefinition>>();
        }}
    }}

    public sealed class Contextual{baseClassName}<TContextDefinition> : IContextualResult<TContextDefinition>
        where TContextDefinition : IContextDefinition
    {{
        public IContext<TContextDefinition> Context {{ get; }}
        public IReadOnlyList<string> Messages {{ get; }}
        public IEnumerable<IErrorInfo<IErrorDefinition>> Errors {{ get; }}

#if NETSTANDARD2_0
        public bool IsSuccess => !Errors.Any();
        public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
#endif
        public Contextual{baseClassName}(IContext<TContextDefinition> context,
                                 IEnumerable<string>? messages = null,
                                 IEnumerable<IErrorInfo<IErrorDefinition>>? errors = null)
        {{
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Messages = (messages ?? Array.Empty<string>()).AsListReadOnly();
            Errors = errors ?? Enumerable.Empty<IErrorInfo<IErrorDefinition>>();
        }}
    }}

    public sealed class Contextual{baseClassName}OfT<TContextDefinition, TValue> : IContextualResult<TContextDefinition, TValue>
        where TContextDefinition : IContextDefinition
    {{
        public IContext<TContextDefinition> Context {{ get; }}
        public TValue Value {{ get; }}
        public IReadOnlyList<string> Messages {{ get; }}
        public IEnumerable<IErrorInfo<IErrorDefinition>> Errors {{ get; }}

#if NETSTANDARD2_0
        public bool IsSuccess => !Errors.Any();
        public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
#endif
        public Contextual{baseClassName}OfT(IContext<TContextDefinition> context,
                                  TValue value,
                                  IEnumerable<string>? messages = null,
                                  IEnumerable<IErrorInfo<IErrorDefinition>>? errors = null)
        {{
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Value = value;
            Messages = (messages ?? Array.Empty<string>()).AsListReadOnly();
            Errors = errors ?? Enumerable.Empty<IErrorInfo<IErrorDefinition>>();
        }}
    }}
}}";
    }
}

// <copyright file="Builders.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// This library contains the canonical source code stubs for Zentient.Abstractions.
// It is intended to be used as a dependency for the Zentient.Analyzers.Templates
// project to ensure all tests use a consistent, compilable baseline.

namespace Zentient.Analyzers.Stubs
{
    /// <summary>
    /// Contains concrete, compliant implementations of the Builder abstractions.
    /// </summary>
    public static class Builders
    {
        public static string GetCompliantImplementations(string ns) =>
$@"
namespace {ns}.Builders
{{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Zentient.Abstractions;
    using Zentient.Abstractions.Codes;
    using Zentient.Abstractions.Codes.Definitions;
    using Zentient.Abstractions.Envelopes;
    using Zentient.Abstractions.Envelopes.Builders;
    using Zentient.Abstractions.Errors;
    using Zentient.Abstractions.Errors.Definitions;
    using Zentient.Abstractions.Metadata;
    using Zentient.Analyzers.Stubs.Common;
    using Zentient.Analyzers.Stubs.Envelopes;
    
    public sealed class EnvelopeBuilderStub<TCode, TError> : IEnvelopeBuilder<TCode, TError>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        private bool _isSuccess;
        private ICode<TCode>? _code;
        private IEnumerable<IErrorInfo<TError>>? _errors;
        private IReadOnlyCollection<string>? _messages;
        private object? _value;
        private IDictionary<string, IReadOnlyCollection<string>>? _headers;
        private Stream? _stream;
        private IMetadata? _metadata;

        public IEnvelopeBuilder<TCode, TError> WithCode(ICode<TCode> code)
        {{
            _code = code;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithErrors(IEnumerable<IErrorInfo<TError>>? errors)
        {{
            _errors = errors;
            _isSuccess = errors == null || !errors.Any();
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithValue(object? value)
        {{
            _value = value;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithValue<TValue>(TValue? value)
        {{
            _value = value;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithHeaders(IDictionary<string, IReadOnlyCollection<string>>? headers)
        {{
            _headers = headers;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithStream(Stream? stream)
        {{
            _stream = stream;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> WithMetadata(IMetadata? metadata)
        {{
            _metadata = metadata;
            return this;
        }}

        public IEnvelopeBuilder<TCode, TError> AddMetadata(string key, object? value)
        {{
            _metadata = _metadata ?? new MetadataStub();
            // Assuming IMetadata has a WithTag method
            return this;
        }}

        public IEnvelope<TCode, TError> Build()
        {{
            if (_stream != null && _headers != null)
            {{
                return new StreamableEnvelopeStub<TCode, TError, object?>(_value, _stream, _isSuccess, _code, _messages, _errors, _metadata);
            }}
            if (_stream != null)
            {{
                return new StreamableEnvelopeStub<TCode, TError, object?>(_value, _stream, _isSuccess, _code, _messages, _errors, _metadata);
            }}
            if (_headers != null)
            {{
                return new HeaderedEnvelopeStub<TCode, TError, object?>(_value, _headers, _isSuccess, _code, _messages, _errors, _metadata);
            }}
            if (_value != null)
            {{
                return new EnvelopeStubOfT<TCode, TError, object?>(_value, _isSuccess, _code, _messages, _errors, _metadata);
            }}
            return new EnvelopeStub<TCode, TError>(_isSuccess, _code, _messages, _errors, _metadata);
        }}
    }}
}}";
    }
}

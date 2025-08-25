// <copyright file="Envelopes.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// This library contains the canonical source code stubs for Zentient.Abstractions.
// It is intended to be used as a dependency for the Zentient.Analyzers.Templates
// project to ensure all tests use a consistent, compilable baseline.

namespace Zentient.Analyzers.Stubs
{
    /// <summary>
    /// Contains concrete, compliant implementations of the Envelopes abstractions.
    /// </summary>
    public static class Envelopes
    {
        public static string GetCompliantImplementations(string ns, string baseClassName) =>
$@"
namespace {ns}.Envelopes
{{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using Zentient.Abstractions;
    using Zentient.Abstractions.Codes;
    using Zentient.Abstractions.Codes.Definitions;
    using Zentient.Abstractions.Common;
    using Zentient.Abstractions.Envelopes;
    using Zentient.Abstractions.Errors;
    using Zentient.Abstractions.Errors.Definitions;
    using Zentient.Abstractions.Metadata;
    using Zentient.Analyzers.Stubs.Common;

    public sealed class {baseClassName}<TCode, TError> : IEnvelope<TCode, TError>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public bool IsSuccess {{ get; }}
        public ICode<TCode> Code {{ get; }}
        public IReadOnlyCollection<string> Messages {{ get; }}
        public IReadOnlyList<IErrorInfo<TError>> Errors {{ get; }}
        public IMetadata Metadata {{ get; }}

        public {baseClassName}(bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
        {{
            IsSuccess = isSuccess;
            Code = code ?? new CodeStub<TCode>(default!);
            Messages = messages ?? new List<string>().AsReadOnly();
            Errors = errors ?? new List<IErrorInfo<TError>>().AsReadOnly();
            Metadata = metadata ?? new MetadataStub();
        }}
    }}

    public sealed class {baseClassName}OfT<TCode, TError, TValue>
        : {baseClassName}<TCode, TError>, IEnvelope<TCode, TError, TValue>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public TValue? Value {{ get; }}

        public {baseClassName}OfT(TValue value, bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
            : base(isSuccess, code, messages, errors, metadata)
        {{
            Value = value;
        }}
    }}
    
    public sealed class Headered{baseClassName}<TCode, TError>
        : {baseClassName}<TCode, TError>, IHeaderedEnvelope<TCode, TError>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers {{ get; }}

        public Headered{baseClassName}(IReadOnlyDictionary<string, IReadOnlyCollection<string>>? headers = null, bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
            : base(isSuccess, code, messages, errors, metadata)
        {{
            Headers = headers ?? new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
        }}
    }}

    public sealed class Headered{baseClassName}OfT<TCode, TError, TValue>
        : {baseClassName}OfT<TCode, TError, TValue>, IHeaderedEnvelope<TCode, TError, TValue>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers {{ get; }}

        public Headered{baseClassName}OfT(TValue value, IReadOnlyDictionary<string, IReadOnlyCollection<string>>? headers = null, bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
            : base(value, isSuccess, code, messages, errors, metadata)
        {{
            Headers = headers ?? new Dictionary<string, IReadOnlyCollection<string>>().AsReadOnly();
        }}
    }}
    
    public sealed class Streamable{baseClassName}<TCode, TError>
        : {baseClassName}<TCode, TError>, IStreamableEnvelope<TCode, TError>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public Stream Stream {{ get; }}

        public Streamable{baseClassName}(Stream stream, bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
            : base(isSuccess, code, messages, errors, metadata)
        {{
            Stream = stream ?? Stream.Null;
        }}
    }}

    public sealed class Streamable{baseClassName}OfT<TCode, TError, TValue>
        : {baseClassName}OfT<TCode, TError, TValue>, IStreamableEnvelope<TCode, TError, TValue>
        where TCode : ICodeDefinition
        where TError : IErrorDefinition
    {{
        public Stream Stream {{ get; }}

        public Streamable{baseClassName}OfT(TValue value, Stream stream, bool isSuccess = true, ICode<TCode>? code = null, IReadOnlyCollection<string>? messages = null, IReadOnlyList<IErrorInfo<TError>>? errors = null, IMetadata? metadata = null)
            : base(value, isSuccess, code, messages, errors, metadata)
        {{
            Stream = stream ?? Stream.Null;
        }}
    }}
}}
";
    }
}

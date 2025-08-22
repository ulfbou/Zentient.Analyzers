// <copyright file="ResultAnalyzerTestConstants.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Zentient.Analyzers.Tests.Diagnostics.Result
{
    public static partial class ResultAnalyzerTestConstants
    {
        // === BASE CONSTANTS ===
        // These constants only contain the necessary using directives
        // for each test category. No dummy implementations are needed.
        private const string BaseUsings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Codes;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Results;
";

        public const string EnvelopeUsings = @"
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Zentient.Abstractions.Codes;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Readers;
using Zentient.Abstractions.Relations;
using Zentient.Abstractions.Relations.Definitions;
using Zentient.Abstractions.Results;
";

        // === TEST CODE CONSTANTS ===
        // Each test constant now only contains the specific code to be analyzed.
        // The base classes and interfaces from Zentient.Abstractions are provided by the test runner.
        public const string MissingMessagesProperty = BaseUsings + @"
public sealed class {0} : IResult
{
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public bool IsSuccess => true;
}
";

        public const string MissingErrorsProperty = BaseUsings + @"
public sealed class {0} : IResult
{
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public bool IsSuccess => true;
}
";

        public const string MissingValueForGeneric = BaseUsings + @"
public sealed class {0} : IResult<int>
{
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public bool IsSuccess => true;
}
";

        public const string IsSuccessWithSetter = BaseUsings + @"
public sealed class {0} : IResult
{
    public bool IsSuccess { get; set; }
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";

        public const string IsSuccessNotDerivedFromErrors = BaseUsings + @"
public sealed class {0} : IResult
{
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";

        public const string IsSuccessDerivedFromErrors = BaseUsings + @"
public sealed class {0} : IResult
{
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";

        public const string EnvelopeIsSuccessWithSetter = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition>
{
    public bool IsSuccess { get; set; }
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string EnvelopeIsSuccessNotDerived = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition>
{
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string EnvelopeIsSuccessDerived = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition>
{
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string EnvelopeMissingCodeProperty = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition>
{
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public IMetadata Metadata { get; }
}
";

        public const string EnvelopeGenericValueWithSetter = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public int Value { get; set; }
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string EnvelopeGenericValueWithPrivateSetter = EnvelopeUsings + @"
public sealed class {0} : IEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public int Value { get; private set; }
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string HeaderedEnvelopeMissingHeadersProperty = EnvelopeUsings + @"
public sealed class {0} : IHeaderedEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public int Value { get; }
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string HeaderedEnvelopeWithHeadersProperty = EnvelopeUsings + @"
public sealed class HeaderedEnvelopeWithHeaders : IHeaderedEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; } = Array.Empty<KeyValuePair<string, IReadOnlyCollection<string>>>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    public int Value { get; } = 0;
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = Array.Empty<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = Array.Empty<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; } = null!;
    public IMetadata Metadata { get; } = null!;
}
";

        public const string StreamableEnvelopeMissingStreamProperty = EnvelopeUsings + @"
public sealed class {0} : IStreamableEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public int Value { get; }
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";

        public const string StreamableEnvelopeWithStreamProperty = EnvelopeUsings + @"
public sealed class {0} : IStreamableEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public Stream Stream { get; }
    public int Value { get; }
    public bool IsSuccess => true;
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; }
    public IMetadata Metadata { get; }
}
";
        public const string FullyCompliantEnvelopeProperties = @"
    public int Value { get; } = 0;
    public bool IsSuccess => Errors?.Count == 0;
    public IReadOnlyCollection<string> Messages { get; } = Array.Empty<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = Array.Empty<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; } = null!;
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; } = Array.Empty<KeyValuePair<string, IReadOnlyCollection<string>>>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    public IMetadata Metadata { get; } = null!;
";
        public const string FactoryProperties = @"
    private {0}() {}
    public static {0} Create () => null!;
";
        public const string StreamableEnvelopeProperty = EnvelopeUsings + @"
public sealed class {0} : IStreamableEnvelope<ICodeDefinition, IErrorDefinition, int>, IHeaderedEnvelope<ICodeDefinition, IErrorDefinition, int>
{
{{FullyCompliantEnvelopeProperties}}
    public Stream Stream { get; } = null!;
{{FactoryProperties}}
}
";

        public const string FullyCompliantEnvelopeBase = EnvelopeUsings + @"
public sealed class {0} : IStreamableEnvelope<ICodeDefinition, IErrorDefinition, int>, IHeaderedEnvelope<ICodeDefinition, IErrorDefinition, int>
{
    public int Value { get; } = 0;
    public bool IsSuccess => Errors?.Count == 0;
    public IReadOnlyCollection<string> Messages { get; } = Array.Empty<string>();
    public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors { get; } = Array.Empty<IErrorInfo<IErrorDefinition>>();
    public ICode<ICodeDefinition> Code { get; } = null!;
    public Stream Stream { get; } = null!;
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; } = Array.Empty<KeyValuePair<string, IReadOnlyCollection<string>>>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    public IMetadata Metadata { get; } = null!;

    private {0}() {} // Required for compilation

    public static {0} Create() => new {0}(); // Required for compilation
}
";

        public const string FullyCompliantResultBase = BaseUsings + @"
public sealed class {0} : IResult
{
    private {0}() { }
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyCollection<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
    public static IResult Create() => new {0}();
}
";
    }
}

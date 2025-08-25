// <copyright file="Common.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// This library contains the canonical source code stubs for Zentient.Abstractions.
// It is intended to be used as a dependency for the Zentient.Analyzers.Templates
// project to ensure all tests use a consistent, compilable baseline.

namespace Zentient.Analyzers.Stubs
{
    /// <summary>
    /// Contains compliant implementations of common, cross-cutting abstractions.
    /// </summary>
    public static class Common
    {
        public static string GetCompliantImplementations(string ns) =>
$@"
namespace {ns}.Common
{{
    // NOTE: This extension method is a substitute for the one in Zentient.Analyzers.Stubs.
    // It must be defined in the generated code to be available for compilation.
    internal static class CollectionsExtensions
    {{
        public static IReadOnlyList<T> AsListReadOnly<T>(this IEnumerable<T> source)
            => (source ?? Array.Empty<T>()).ToList().AsReadOnly();
    }}

    public sealed class MetadataStub : IMetadata
    {{
        private readonly Dictionary<string, object?> _tags = new();

        public MetadataStub(IDictionary<string, object?>? tags = null)
        {{
            if (tags != null)
            {{
                foreach (var tag in tags) _tags[tag.Key] = tag.Value;
            }}
        }}

        // IMetadataReader implementation
        public int Count => _tags.Count;
        public IEnumerable<string> Keys => _tags.Keys;
        public IEnumerable<object?> Values => _tags.Values;
        public IEnumerable<KeyValuePair<string, object?>> Tags => _tags;
        public bool ContainsKey(string key) => _tags.ContainsKey(key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value) => _tags.TryGetValue(key, out value);
        public TValue GetValueOrDefault<TValue>(string key, TValue defaultValue = default!) =>
            _tags.TryGetValue(key, out var v) && v is TValue typedValue ? typedValue : defaultValue;
        public bool TryGetValue<TValue>(string key, [MaybeNullWhen(false)] out TValue value)
        {{
            if (_tags.TryGetValue(key, out var v) && v is TValue typedValue)
            {{
                value = typedValue;
                return true;
            }}
            value = default;
            return false;
        }}

        // IMetadata implementation
        public IMetadata WithTag(string key, object? value) =>
            new MetadataStub(new Dictionary<string, object?>(_tags) {{ [key] = value }});
        public IMetadata WithoutTag(string key)
        {{
            var newTags = new Dictionary<string, object?>(_tags);
            newTags.Remove(key);
            return new MetadataStub(newTags);
        }}
        public IMetadata WithTag(Func<KeyValuePair<string, object?>> tagFactory) => WithTag(tagFactory().Key, tagFactory().Value);
        public IMetadata WithTags(IEnumerable<KeyValuePair<string, object?>> tags)
        {{
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var tag in tags) newTags[tag.Key] = tag.Value;
            return new MetadataStub(newTags);
        }}
        public IMetadata WithTags(Func<IEnumerable<KeyValuePair<string, object?>>> tagsFactory) => WithTags(tagsFactory());
        public IMetadata WithoutTags(IEnumerable<string> keys)
        {{
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var key in keys) newTags.Remove(key);
            return new MetadataStub(newTags);
        }}
        public IMetadata WithoutTag(Func<string, bool> keyPredicate)
        {{
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var key in newTags.Keys.ToList())
            {{
                if (keyPredicate(key)) newTags.Remove(key);
            }}
            return new MetadataStub(newTags);
        }}
        public IMetadata Merge(IMetadataReader other) => WithTags(other.Tags);
    }}

    public sealed class ErrorDefinitionStub : IErrorDefinition
    {{
        public ErrorSeverity Severity {{ get; }}
        public bool IsTransient {{ get; }}
        public bool IsUserFacing {{ get; }}
        public string Id {{ get; }}
        public string Name {{ get; }}
        public string Version {{ get; }}
        public string Description {{ get; }}
        public string DisplayName {{ get; }}
        public string CategoryName {{ get; }}
        public IRelationCategory? Category {{ get; }}
        public IReadOnlyCollection<IRelationDefinition> Relations {{ get; }}
        public IMetadata Metadata {{ get; }}

        public ErrorDefinitionStub(string id = ""E001"", string name = ""DefaultError"")
        {{
            Id = id;
            Name = name;
            Severity = ErrorSeverity.Error;
            IsTransient = false;
            IsUserFacing = true;
            Version = ""1.0.0"";
            Description = ""A default error for testing."";
            DisplayName = name;
            CategoryName = ""General"";
            Category = null;
            Relations = Array.Empty<IRelationDefinition>();
            Metadata = new MetadataStub();
        }}
    }}
    
    public sealed class ErrorInfoStub<TError> : IErrorInfo<TError>
        where TError : IErrorDefinition
    {{
        public TError ErrorDefinition {{ get; }}
        public ICode<ICodeDefinition> Code {{ get; }}
        public string Message {{ get; }}
        public string InstanceId {{ get; }}
        public IReadOnlyCollection<IErrorInfo<TError>> InnerErrors {{ get; }}
        public IMetadata Metadata {{ get; }}

        public ErrorInfoStub(TError errorDefinition, string message = ""An error occurred."", string instanceId = ""123"")
        {{
            ErrorDefinition = errorDefinition;
            Code = new CodeStub<ICodeDefinition>(new CodeDefinitionStub(""C001""));
            Message = message;
            InstanceId = instanceId;
            InnerErrors = Array.Empty<IErrorInfo<TError>>().AsListReadOnly();
            Metadata = new MetadataStub();
        }}
    }}
    
    public sealed class CodeDefinitionStub : ICodeDefinition
    {{
        public string Id {{ get; }}
        public string Name {{ get; }}
        public string Version {{ get; }}
        public string Description {{ get; }}
        public string DisplayName {{ get; }}
        public string CategoryName {{ get; }}
        public IRelationCategory? Category {{ get; }}
        public IReadOnlyCollection<IRelationDefinition> Relations {{ get; }}
        public IMetadata Metadata {{ get; }}

        public CodeDefinitionStub(string id = ""C001"", string name = ""DefaultCode"")
        {{
            Id = id;
            Name = name;
            Version = ""1.0.0"";
            Description = ""A default code for testing."";
            DisplayName = name;
            CategoryName = ""General"";
            Category = null;
            Relations = Array.Empty<IRelationDefinition>();
            Metadata = new MetadataStub();
        }}
    }}
    
    public sealed class CodeStub<TCode> : ICode<TCode> where TCode : ICodeDefinition
    {{
        public TCode Definition {{ get; }}
        public IMetadata Metadata {{ get; }}
        public CodeStub(TCode definition)
        {{
            Definition = definition;
            Metadata = new MetadataStub();
        }}
    }}
    
    public sealed class ContextDefinitionStub : IContextDefinition
    {{
        public string Id {{ get; }}
        public string Name {{ get; }}
        public string Version {{ get; }}
        public string Description {{ get; }}
        public string DisplayName {{ get; }}
        public string CategoryName {{ get; }}
        public IRelationCategory? Category {{ get; }}
        public IReadOnlyCollection<IRelationDefinition> Relations {{ get; }}
        public IMetadata Metadata {{ get; }}

        public ContextDefinitionStub(string id = ""CTX001"", string name = ""DefaultContext"")
        {{
            Id = id;
            Name = name;
            Version = ""1.0.0"";
            Description = ""A default context definition."";
            DisplayName = name;
            CategoryName = ""General"";
            Category = null;
            Relations = Array.Empty<IRelationDefinition>();
            Metadata = new MetadataStub();
        }}
    }}
    
    public sealed class ContextStub<TContextDefinition> : IContext<TContextDefinition>
        where TContextDefinition : IContextDefinition
    {{
        public string Name {{ get; }}
        public TContextDefinition Definition {{ get; }}
        public IMetadata Metadata {{ get; }}
        public ContextStub(string name, TContextDefinition definition)
        {{
            Name = name;
            Definition = definition;
            Metadata = new MetadataStub();
        }}
    }}
    internal static class CollectionsExt
    {{
        public static IReadOnlyList<T> AsListReadOnly<T>(this IEnumerable<T> source)
            => (source ?? Array.Empty<T>()).ToList();
    }}
}}
";
    }
}

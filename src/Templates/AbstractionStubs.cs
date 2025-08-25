// <copyright file="AbstractionStubs.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Zentient.Analyzers.Templates
{
    public static class AbstractionStubs
    {
        public const string All = @"
// ========================================================================================
// Zentient.Abstractions.Common
// ========================================================================================
namespace Zentient.Abstractions.Common
{
    public interface IHasMetadata { Zentient.Abstractions.Metadata.IMetadata Metadata { get; } }
    public interface IHasCategory
    {
        string CategoryName { get; }
        Zentient.Abstractions.Relations.IRelationCategory? Category { get; }
    }
    public interface IHasDescription { string Description { get; } }
    public interface IHasName { string Name { get; } }
    public interface IHasVersion { string Version { get; } }
    public interface IIdentifiable { string Id { get; } }
    public interface IHasParent<out TParent> { TParent? Parent { get; } }
}

// ========================================================================================
// Zentient.Abstractions.Common.Definitions
// ========================================================================================
namespace Zentient.Abstractions.Common.Definitions
{
    public interface IDefinition { }
    public interface IIdentifiableDefinition : IDefinition, Zentient.Abstractions.Common.IIdentifiable { }
    public interface ITypeDefinition :
        IIdentifiableDefinition,
        Zentient.Abstractions.Common.IHasName,
        Zentient.Abstractions.Common.IHasVersion,
        Zentient.Abstractions.Common.IHasDescription,
        Zentient.Abstractions.Common.IHasCategory,
        Zentient.Abstractions.Relations.IHasRelation,
        Zentient.Abstractions.Common.IHasMetadata
    { }
}

// ========================================================================================
// Zentient.Abstractions.Codes
// ========================================================================================
namespace Zentient.Abstractions.Codes.Definitions
{
    public interface ICodeDefinition : Zentient.Abstractions.Common.Definitions.ITypeDefinition { }
}

namespace Zentient.Abstractions.Codes
{
    public interface ICode<out TCodeDefinition> : Zentient.Abstractions.Common.IHasMetadata
        where TCodeDefinition : ICodeDefinition
    {
        TCodeDefinition Definition { get; }
    }
}

// ========================================================================================
// Zentient.Abstractions.Errors
// ========================================================================================
namespace Zentient.Abstractions.Errors.Definitions
{
    public interface IErrorDefinition : Zentient.Abstractions.Common.Definitions.ITypeDefinition
    {
        ErrorSeverity Severity { get; }
        bool IsTransient { get; }
        bool IsUserFacing { get; }
    }
    public enum ErrorSeverity { Informational, Warning, Error, Critical }
}

namespace Zentient.Abstractions.Errors
{
    public interface IErrorInfo<out TErrorDefinition> : Zentient.Abstractions.Common.IHasMetadata
        where TErrorDefinition : IErrorDefinition
    {
        TErrorDefinition ErrorDefinition { get; }
        Zentient.Abstractions.Codes.ICode<Zentient.Abstractions.Codes.Definitions.ICodeDefinition> Code { get; }
        string Message { get; }
        string InstanceId { get; }
        IReadOnlyCollection<IErrorInfo<TErrorDefinition>> InnerErrors { get; }
    }
}

// ========================================================================================
// Zentient.Abstractions.Metadata
// ========================================================================================
namespace Zentient.Abstractions.Metadata.Readers
{
    public interface IMetadataReader
    {
        int Count { get; }
        IEnumerable<string> Keys { get; }
        IEnumerable<object?> Values { get; }
        IEnumerable<KeyValuePair<string, object?>> Tags { get; }
        bool ContainsKey(string key);
#if NETSTANDARD2_0
        bool TryGetValue(string key, [NotNullWhen(true)] out object? value);
#else
        bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value);
#endif
#if NETSTANDARD2_0
        bool TryGetValue<TValue>(string key, [NotNullWhen(true)] out TValue value) where TValue : class;
#else
        bool TryGetValue<TValue>(string key, [MaybeNullWhen(false)] out TValue value);
#endif
        object? GetValueOrDefault(string key, object? defaultValue = default);
        TValue GetValueOrDefault<TValue>(string key, TValue defaultValue = default!);
    }
}

namespace Zentient.Abstractions.Metadata
{
    public interface IMetadata : IMetadataReader
    {
        IMetadata WithTag(string key, object? value);
        IMetadata WithoutTag(string key);
        IMetadata WithTag(Func<KeyValuePair<string, object?>> tagFactory);
        IMetadata WithTags(IEnumerable<KeyValuePair<string, object?>> tags);
        IMetadata WithTags(Func<IEnumerable<KeyValuePair<string, object?>>> tagsFactory);
        IMetadata WithoutTags(IEnumerable<string> keys);
        IMetadata WithoutTag(Func<string, bool> keyPredicate);
        IMetadata Merge(IMetadataReader other);
    }
}

// ========================================================================================
// Zentient.Abstractions.Relations
// ========================================================================================
namespace Zentient.Abstractions.Relations
{
    public interface IRelationCategory
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
    public interface IHasRelation { IReadOnlyCollection<Zentient.Abstractions.Relations.Definitions.IRelationDefinition> Relations { get; } }
}

namespace Zentient.Abstractions.Relations.Definitions
{
    public interface IRelationDefinition : Zentient.Abstractions.Common.Definitions.ITypeDefinition, Zentient.Abstractions.Common.IHasParent<IRelationDefinition> { }
}

// ========================================================================================
// Zentient.Abstractions.Contexts
// ========================================================================================
namespace Zentient.Abstractions.Contexts.Definitions
{
    public interface IContextDefinition : Zentient.Abstractions.Common.Definitions.ITypeDefinition { }
}

namespace Zentient.Abstractions.Contexts
{
    public interface IContext<out TContextDefinition> : Zentient.Abstractions.Common.IHasMetadata
        where TContextDefinition : IContextDefinition
    {
        TContextDefinition Definition { get; }
        string Name { get; }
    }
}
";
    }
}

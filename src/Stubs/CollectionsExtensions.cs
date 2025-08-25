// <copyright file="CollectionsExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// This library contains the canonical source code stubs for Zentient.Abstractions.
// It is intended to be used as a dependency for the Zentient.Analyzers.Templates
// project to ensure all tests use a consistent, compilable baseline.

namespace Zentient.Analyzers.Stubs
{
    internal static class CollectionsExtensions
    {
        public static IReadOnlyList<T> AsListReadOnly<T>(this IEnumerable<T> source)
            => (source ?? Array.Empty<T>()).ToList();
    }
}

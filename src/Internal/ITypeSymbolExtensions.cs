// <copyright file="ITypeSymbolExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

using Zentient.Analyzers.Diagnostics;
using Zentient.Analyzers.Diagnostics.Immutability;

namespace Zentient.Analyzers.Internal
{
    /// <summary>
    /// Extension methods for <see cref="ITypeSymbol"/> and <see cref="INamedTypeSymbol"/>.
    /// </summary>
    internal static class ITypeSymbolExtensions
    {
        /// <summary>
        /// Determines whether the <paramref name="fromType"/> is assignable to the <paramref name="toType"/>.
        /// </summary>
        /// <param name="fromType">The source type symbol.</param>
        /// <param name="toType">The target type symbol.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="fromType"/> is assignable to <paramref name="toType"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAssignableTo(this ITypeSymbol fromType, ITypeSymbol toType)
            => fromType.AllInterfaces.Contains(toType, SymbolEqualityComparer.Default) ||
               fromType.BaseType?.Equals(toType, SymbolEqualityComparer.Default) == true ||
               SymbolEqualityComparer.Default.Equals(fromType, toType);

        /// <summary>
        /// Determines whether the specified <see cref="INamedTypeSymbol"/> implements any of the well-known immutable abstractions.
        /// </summary>
        /// <param name="type">The named type symbol to check.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="type"/> implements any interface listed in <see cref="WellKnown.ImmutableAbstractions"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ImplementsImmutableAbstraction(this INamedTypeSymbol type)
            => WellKnown.ImplementsAnyOf(type, WellKnown.ImmutableAbstractions.ToArray());

        /// <summary>
        /// Determines whether the specified type implements the well-known abstraction.
        /// </summary>
        /// <param name="type">The named type symbol to check.</param>
        /// <param name="fullyQualifiedInterfaceName">The fully qualified name of the interface to check for.</param>
        /// <returns>
        /// <see langword="true"/> if the type implements the interface; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Implements(this INamedTypeSymbol type, string fullyQualifiedInterfaceName)
            => type.AllInterfaces.Any(i => i.ToDisplayString() == fullyQualifiedInterfaceName);

        /// <summary>
        /// Determines whether the specified <see cref="ITypeSymbol"/> implements the interface identified by <paramref name="validationContext"/>.
        /// </summary>
        /// <param name="returnType">The type symbol to check.</param>
        /// <param name="validationContext">The fully qualified name of the interface to check for.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="returnType"/> is a named type that implements the interface specified by <paramref name="validationContext"/> and has no type arguments; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Implements(this ITypeSymbol returnType, string validationContext)
            => returnType is INamedTypeSymbol namedType &&
               namedType.Implements(validationContext) &&
               namedType.TypeArguments.Length == 0;
    }
}

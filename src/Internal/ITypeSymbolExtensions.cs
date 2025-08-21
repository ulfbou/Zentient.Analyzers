// <copyright file="ITypeSymbolExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

using Zentient.Analyzers.Diagnostics.Immutability;

namespace Zentient.Analyzers.Internal
{
    /// <summary>
    /// Extension methods for <see cref="INamedTypeSymbol"/> and <see cref="ITypeSymbol"/> to assist with immutability analysis.
    /// </summary>
    internal static class ITypeSymbolExtensions
    {
        /// <summary>
        /// Determines whether the specified type implements any of the well-known immutable abstractions.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <returns>
        /// <c>true</c> if the type implements any immutable abstraction; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImmutableAbstraction(this INamedTypeSymbol type)
            => WellKnown.ImplementsAnyOf(type, WellKnown.ImmutableAbstractions.ToArray());

        /// <summary>
        /// Determines whether the specified type is a validation context.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <returns>
        /// <c>true</c> if the type is a validation context; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidationContext(this INamedTypeSymbol type)
            => type.ToDisplayString() == WellKnown.IValidationContext;

        /// <summary>
        /// Determines whether the <paramref name="fromType"/> is assignable to the <paramref name="toType"/>.
        /// </summary>
        /// <param name="fromType">The source type symbol.</param>
        /// <param name="toType">The target type symbol.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="type"/> is assignable to <paramref name="returnType"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAssignableTo(this ITypeSymbol fromType, ITypeSymbol toType)
            => fromType.AllInterfaces.Contains(toType, SymbolEqualityComparer.Default) ||
               fromType.BaseType?.Equals(toType, SymbolEqualityComparer.Default) == true ||
               SymbolEqualityComparer.Default.Equals(fromType, toType);
    }
}

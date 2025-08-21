// <copyright file="INamedTypeSymbolExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Zentient.Analyzers.Internal
{
    /// <summary>Extension methods for <see cref="INamedTypeSymbol"/>.</summary>
    internal static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Determines whether the <paramref name="type"/> is assignable to the specified <paramref name="returnType"/>.
        /// </summary>
        /// <param name="type">The source named type symbol.</param>
        /// <param name="returnType">The target type symbol.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="type"/> is assignable to <paramref name="returnType"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAssignableTo(this INamedTypeSymbol type, ITypeSymbol returnType)
            => type.AllInterfaces.Contains(returnType, SymbolEqualityComparer.Default) ||
               type.BaseType?.Equals(returnType, SymbolEqualityComparer.Default) == true ||
               SymbolEqualityComparer.Default.Equals(type, returnType);
    }
}

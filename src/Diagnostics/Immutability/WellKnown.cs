// <copyright file="WellKnown.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;

namespace Zentient.Analyzers.Diagnostics.Immutability
{
    internal static class WellKnown
    {
        // Immutable abstraction interfaces to be checked by the analyzer
        public static readonly ImmutableArray<string> ImmutableAbstractions = ImmutableArray.Create(
            "Zentient.Abstractions.Results.IResult",
            "Zentient.Abstractions.Results.IEnvelope",
            "Zentient.Abstractions.Metadata.IMetadata",
            "Zentient.Abstractions.Errors.IErrorInfo",
            "Zentient.Abstractions.Codes.ICode",
            "Zentient.Abstractions.Validation.IValidationContext"
        );

        // Specific interfaces that lack a builder and require static factory methods
        public const string IValidationContext = "Zentient.Abstractions.Validation.IValidationContext";

        /// <summary>
        /// Checks if a type symbol implements any of the specified interfaces.
        /// </summary>
        public static bool ImplementsAnyOf(
            INamedTypeSymbol type,
            params string[] interfaceNames)
        {
            foreach (var iface in type.AllInterfaces)
            {
                foreach (var name in interfaceNames)
                {
                    if (iface.ToDisplayString().StartsWith(name, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

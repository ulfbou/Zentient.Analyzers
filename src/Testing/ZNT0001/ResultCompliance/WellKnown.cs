// <copyright file="WellKnown.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Zentient.Analyzers.Testing.ZNT0001.ResultCompliance
{
    internal static partial class WellKnown
    {
        public const string IResult = "Zentient.Abstractions.Results.IResult";
        public const string IResultOfT = "Zentient.Abstractions.Results.IResult`1";
        public const string IErrorInfoOfT = "Zentient.Abstractions.Errors.IErrorInfo`1";
        public const string IErrorDefinition = "Zentient.Abstractions.Errors.Definitions.IErrorDefinition";
        public const string MessagesProp = "Messages";
        public const string ErrorsProp = "Errors";
        public const string IsSuccessProp = "IsSuccess";
        public const string ValueProp = "Value";

        public static bool ImplementsIResult(
            INamedTypeSymbol type,
            Compilation compilation,
            out INamedTypeSymbol? resultInterface,
            out INamedTypeSymbol? resultOfTInterface)
        {
            resultInterface = compilation.GetTypeByMetadataName(IResult);
            var resultOfT = compilation.GetTypeByMetadataName(IResultOfT);
            resultOfTInterface = null;

            if (resultInterface is null && resultOfT is null)
                return false;

            foreach (var iface in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(iface.OriginalDefinition, resultOfT))
                {
                    resultOfTInterface = iface;
                    return true;
                }

                if (SymbolEqualityComparer.Default.Equals(iface, resultInterface))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

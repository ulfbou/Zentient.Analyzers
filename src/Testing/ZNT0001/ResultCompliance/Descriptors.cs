// <copyright file="Descriptors.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Zentient.Analyzers.Testing.ZNT0001.ResultCompliance
{
    internal static partial class Descriptors
    {
        private const string Category = "Zentient.ResultDoctrine";
        public static readonly DiagnosticDescriptor NoSettableIsSuccess = new(
            id: "ZNT0001",
            title: "Result types must compute success from Errors (no settable IsSuccess)",
            messageFormat: "Type '{0}' implements IResult; property 'IsSuccess' must be get-only and computed, not settable",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Zentient doctrine: IsSuccess is a derived property computed from Errors; implementations must not expose a setter.");

        public static DiagnosticDescriptor MissingMessages => missingMessages;
        private static readonly DiagnosticDescriptor missingMessages = new(
            id: "ZNT0001A",
            title: "Result types must expose readable Messages",
            messageFormat: "Type '{0}' implements IResult but is missing a readable 'Messages' property",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Zentient doctrine requires a readable Messages collection on all Result types.");

        public static readonly DiagnosticDescriptor MissingErrors = new(
            id: "ZNT0001B",
            title: "Result types must expose readable Errors",
            messageFormat: "Type '{0}' implements IResult but is missing a readable 'Errors' property",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Zentient doctrine requires a readable Errors sequence on all Result types.");

        public static DiagnosticDescriptor MissingValueForGeneric => missingValueForGeneric;
        private static readonly DiagnosticDescriptor missingValueForGeneric = new(
            id: "ZNT0001C",
            title: "IResult<T> types must expose readable Value",
            messageFormat: "Type '{0}' implements IResult<T> but is missing a readable 'Value' property",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Zentient doctrine requires a readable Value on generic Result types.");
    }
}

// <copyright file="Descriptors.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Zentient.Analyzers
{
    internal static partial class Descriptors
    {
        private const string ImmutabilityCategory = "Zentient.Immutability";
        private const string TestingCategory = "Zentient.Testing";
        private const string DeveloperExperienceCategory = "Zentient.DeveloperExperience";
        private const string ObservabilityCategory = "Zentient.Observability";
        private const string ComponentLifecycleCategory = "Zentient.ComponentLifecycle";

        public static readonly DiagnosticDescriptor ZNT1002_IsSuccessDerived = new(
            id: "ZNT1002",
            title: "IsSuccess must be a derived, computed property",
            messageFormat: "The IsSuccess property on type '{0}' must not have a setter and its value must be derived from the Errors collection",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The IsSuccess property is a reflection of the Errors collection's state, not an independent piece of data. This ensures consistency.");

        public static readonly DiagnosticDescriptor ZNT2001_TestHarnessLifecycle = new(
            id: "ZNT2001",
            title: "Test harness must be used exactly once per instance",
            messageFormat: "The RunAsync() method on ITestHarness instance '{0}' was called more than once",
            category: TestingCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the 'single-use, isolated test' principle to prevent side effects and ensure reproducibility.");

        public static readonly DiagnosticDescriptor ZNT2002_FullMockCoverage = new(
            id: "ZNT2002",
            title: "Mock verifier must have a corresponding assertion",
            messageFormat: "The mock verifier instance '{0}' was created but no assertion was made to verify its behavior",
            category: TestingCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that a mock's configured behavior is actually being tested, preventing 'dead' mocking logic.");

        public static readonly DiagnosticDescriptor ZNT3001_FluentAssertionUsage = new(
            id: "ZNT3001",
            title: "Use fluent assertions for IResult and IEnvelope",
            messageFormat: "Direct property access on '{0}' should be replaced with a fluent assertion method like BeSuccess() or HaveValue()",
            category: DeveloperExperienceCategory,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Ensures consistent API usage and leverages the human-readable failure messages provided by the testing library.");

        public static readonly DiagnosticDescriptor ZNT3002_BuilderMethodChaining = new(
            id: "ZNT3002",
            title: "Builder methods must return the builder instance",
            messageFormat: "Method '{0}' on a builder type does not return a builder instance, which breaks fluent chaining",
            category: DeveloperExperienceCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Reinforces the fluent, declarative API style by ensuring all builder methods enable method chaining.");

        public static readonly DiagnosticDescriptor ZNT4001_FailureObservation = new(
            id: "ZNT4001",
            title: "Result failures must be logged or propagated",
            messageFormat: "Method '{0}' returns a failing IResult without logging the failure via an injected ILogger",
            category: ObservabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the 'no silent failures' principle, ensuring all failures are observed by the system's observability pipeline.");

        public static readonly DiagnosticDescriptor ZNT5001_HandlerPolicyStatelessness = new(
            id: "ZNT5001",
            title: "Handlers and policies must be stateless",
            messageFormat: "Type '{0}' contains a mutable, non-injected, non-const instance field '{1}', which violates statelessness",
            category: ComponentLifecycleCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures handlers and policies are thread-safe and can be registered with a Transient or Singleton lifetime without concurrency issues.");

        public static readonly ImmutableArray<DiagnosticDescriptor> AllDiagnostics = ImmutableArray.Create(
            ZNT1001A_ConcreteTypeMustBeSealed,
            ZNT1001B_PropertiesMustBeGetOnly,
            ZNT1001C_NoPublicConstructors,
            ZNT1001D_MustHaveStaticFabricMethods,
            ZNT1002_IsSuccessDerived,
            ZNT2001_TestHarnessLifecycle,
            ZNT2002_FullMockCoverage,
            ZNT3001_FluentAssertionUsage,
            ZNT3002_BuilderMethodChaining,
            ZNT4001_FailureObservation,
            ZNT5001_HandlerPolicyStatelessness);
    }
}

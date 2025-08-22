// <copyright file="Descriptors.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Zentient.Analyzers.Diagnostics
{
    public static partial class Descriptors
    {
        private const string ImmutabilityCategory = "Zentient.ImmutabilityDoctrine";
        private const string ResultDoctrineCategory = "Zentient.ResultDoctrine";
        private const string EnvelopeDoctrineCategory = "Zentient.EnvelopeDoctrine";
        private const string CodeDoctrineCategory = "Zentient.CodeDoctrine";
        private const string ErrorDoctrineCategory = "Zentient.ErrorDoctrine";
        private const string ValidationDoctrineCategory = "Zentient.ValidationDoctrine";
        private const string MetadataDoctrineCategory = "Zentient.MetadataDoctrine";

        // ZNT0002-ZNT0005: Result Doctrine
        public static DiagnosticDescriptor ZNT0002IsSuccessDerivedFromErrors => isSuccessDerivedFromErrors;
        private static readonly DiagnosticDescriptor isSuccessDerivedFromErrors = new(
            id: "ZNT0002",
            title: "IsSuccess must be derived from the Errors collection",
            messageFormat: "The 'IsSuccess' property on type '{0}' must be a computed property derived from the state of the 'Errors' collection",
            category: ResultDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures consistency by preventing a mismatch between the result's success state and the presence of errors.");

        public static DiagnosticDescriptor ZNT0003MissingMessagesProperty => missingMessagesProperty;
        private static readonly DiagnosticDescriptor missingMessagesProperty = new(
            id: "ZNT0003",
            title: "Result type must contain a Messages property",
            messageFormat: "The result type '{0}' must contain a public or internal get-only 'Messages' property",
            category: ResultDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the presence of a 'Messages' property for providing diagnostic or informational context on a result.");

        public static DiagnosticDescriptor ZNT0004MissingErrorsProperty => missingErrorsProperty;
        private static readonly DiagnosticDescriptor missingErrorsProperty = new(
            id: "ZNT0004",
            title: "Result type must contain an Errors property",
            messageFormat: "The result type '{0}' must contain a public or internal get-only 'Errors' property",
            category: ResultDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the presence of an 'Errors' collection, which is fundamental for structured failure reporting and deriving the 'IsSuccess' state.");

        public static DiagnosticDescriptor ZNT0005MissingValueForGeneric => missingValueForGeneric;
        private static readonly DiagnosticDescriptor missingValueForGeneric = new(
            id: "ZNT0005",
            title: "Generic result must contain a Value property",
            messageFormat: "The generic result type '{0}' must contain a public or internal get-only 'Value' property",
            category: ResultDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that generic results and envelopes expose their encapsulated value, which is a core part of their contract.");

        // ZNT0006-ZNT0009: Immutability Doctrine
        public static DiagnosticDescriptor ZNT0006ConcreteTypeMustBeSealed => concreteTypeMustBeSealed;
        private static readonly DiagnosticDescriptor concreteTypeMustBeSealed = new(
            id: "ZNT0006",
            title: "Concrete immutable type must be sealed",
            messageFormat: "Concrete type '{0}' implementing a core immutable abstraction must be declared 'sealed'",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the immutability and encapsulation of core data contracts by requiring their concrete implementations to be sealed. This prevents unintended modification of object state through inheritance.");

        public static DiagnosticDescriptor ZNT0007PropertiesMustBeGetOnly => propertiesMustBeGetOnly;
        private static readonly DiagnosticDescriptor propertiesMustBeGetOnly = new(
            id: "ZNT0007",
            title: "Properties must be get-only",
            messageFormat: "Property '{0}' on type '{1}' must be get-only",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces immutability by preventing external code from altering the state of an object after it has been created.");

        public static DiagnosticDescriptor ZNT0008NoPublicOrInternalConstructors => noPublicOrInternalConstructors;
        private static readonly DiagnosticDescriptor noPublicOrInternalConstructors = new(
            id: "ZNT0008",
            title: "Use a builder or factory instead of a public or internal constructor",
            messageFormat: "Type '{0}' must not contain a public or internal constructor",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces a consistent creation pattern for immutable types, directing developers toward designated builders or static factory methods.");

        public static DiagnosticDescriptor ZNT0009MustHaveStaticFactoryMethods => mustHaveStaticFactoryMethods;
        private static readonly DiagnosticDescriptor mustHaveStaticFactoryMethods = new(
            id: "ZNT0009",
            title: "Type must provide static factory methods",
            messageFormat: "Type '{0}' must provide static factory methods for creation instead of public constructors",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures that types lacking a dedicated builder are created through controlled, static factory methods, promoting immutability and encapsulation.");

        // ZNT0010-ZNT0014: Envelope Doctrine
        public static DiagnosticDescriptor ZNT0010IsSuccessDerivedFromErrorsOrCode => isSuccessDerivedFromErrorsOrCode;
        private static readonly DiagnosticDescriptor isSuccessDerivedFromErrorsOrCode = new(
            id: "ZNT0010",
            title: "IsSuccess must be derived from Errors or Code",
            messageFormat: "The 'IsSuccess' property on type '{0}' must be a computed property derived from either the 'Errors' collection or the 'Code' property.",
            category: EnvelopeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The 'IsSuccess' property for an IEnvelope must be derived from a canonical source to prevent inconsistencies.");

        public static DiagnosticDescriptor ZNT0011MissingCodeProperty => missingCodeProperty;
        private static readonly DiagnosticDescriptor missingCodeProperty = new(
            id: "ZNT0011",
            title: "Envelope type must contain a Code property",
            messageFormat: "The envelope type '{0}' must contain a public or internal get-only 'Code' property",
            category: EnvelopeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The 'Code' property is the primary, type-safe indicator of an IEnvelope's outcome.");

        public static DiagnosticDescriptor ZNT0012ValueMustOnlyExistOnSuccess => valueMustOnlyExistOnSuccess;
        private static readonly DiagnosticDescriptor valueMustOnlyExistOnSuccess = new(
            id: "ZNT0012",
            title: "Envelope value must be null on failure",
            messageFormat: "The 'Value' property on type '{0}' must be null if 'IsSuccess' is false",
            category: EnvelopeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that a value is only present on a successful envelope, maintaining doctrinal consistency.");

        public static DiagnosticDescriptor ZNT0013MissingHeadersPropertyForHeaderedEnvelope => missingHeadersPropertyForHeaderedEnvelope;
        private static readonly DiagnosticDescriptor missingHeadersPropertyForHeaderedEnvelope = new(
            id: "ZNT0013",
            title: "IHeaderedEnvelope must contain a Headers property",
            messageFormat: "The headered envelope type '{0}' must contain a public or internal get-only 'Headers' property",
            category: EnvelopeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the presence of a 'Headers' collection, which is a key part of the IHeaderedEnvelope contract.");

        public static DiagnosticDescriptor ZNT0014MissingStreamPropertyForStreamableEnvelope => missingStreamPropertyForStreamableEnvelope;
        private static readonly DiagnosticDescriptor missingStreamPropertyForStreamableEnvelope = new(
            id: "ZNT0014",
            title: "IStreamableEnvelope must contain a Stream property",
            messageFormat: "The streamable envelope type '{0}' must contain a public or internal get-only 'Stream' property",
            category: EnvelopeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the presence of a 'Stream' property for streamable envelopes.");

        // ZNT0015: Code Doctrine
        public static DiagnosticDescriptor ZNT0015MissingDefinitionPropertyOnCode => missingDefinitionPropertyOnCode;
        private static readonly DiagnosticDescriptor missingDefinitionPropertyOnCode = new(
            id: "ZNT0015",
            title: "ICode must contain a Definition property",
            messageFormat: "The code type '{0}' must contain a public or internal get-only 'Definition' property",
            category: CodeDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that all ICode implementations expose their semantic definition, providing an immutable link to the code's metadata.");

        // ZNT0016-ZNT0020: Error Doctrine
        public static DiagnosticDescriptor ZNT0016MustHaveErrorDefinitionProperty => mustHaveErrorDefinitionProperty;
        private static readonly DiagnosticDescriptor mustHaveErrorDefinitionProperty = new(
            id: "ZNT0016",
            title: "IErrorInfo must have an ErrorDefinition property",
            messageFormat: "The error info type '{0}' must contain a public or internal get-only 'ErrorDefinition' property",
            category: ErrorDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that all 'IErrorInfo' implementations link to their canonical 'IErrorDefinition'.");

        public static DiagnosticDescriptor ZNT0017MustHaveCodeAndMessageProperties => mustHaveCodeAndMessageProperties;
        private static readonly DiagnosticDescriptor mustHaveCodeAndMessageProperties = new(
            id: "ZNT0017",
            title: "IErrorInfo must have Code and Message properties",
            messageFormat: "The error info type '{0}' must contain public or internal get-only 'Code' and 'Message' properties",
            category: ErrorDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that a structured error provides both a semantic code and a human-readable message.");

        public static DiagnosticDescriptor ZNT0018MustHaveInstanceIdProperty => mustHaveInstanceIdProperty;
        private static readonly DiagnosticDescriptor mustHaveInstanceIdProperty = new(
            id: "ZNT0018",
            title: "IErrorInfo must have an InstanceId property",
            messageFormat: "The error info type '{0}' must contain a public or internal get-only 'InstanceId' property",
            category: ErrorDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures traceability for every error instance, which is crucial for correlation across systems.");

        public static DiagnosticDescriptor ZNT0019InnerErrorsCollectionNotNullable => innerErrorsCollectionNotNullable;
        private static readonly DiagnosticDescriptor innerErrorsCollectionNotNullable = new(
            id: "ZNT0019",
            title: "InnerErrors collection must not be nullable",
            messageFormat: "The 'InnerErrors' collection on type '{0}' must be non-nullable and non-null",
            category: ErrorDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Prevents null collections, ensuring a consistent and predictable API for fluent consumption patterns.");

        public static DiagnosticDescriptor ZNT0020RequiredErrorInfoBuilderPropertiesSet => requiredErrorInfoBuilderPropertiesSet;
        private static readonly DiagnosticDescriptor requiredErrorInfoBuilderPropertiesSet = new(
            id: "ZNT0020",
            title: "Builder properties must be set before Build()",
            messageFormat: "Mandatory properties must be set on builder '{0}' before calling Build()",
            category: ErrorDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Prevents the creation of incomplete or invalid 'IErrorInfo' instances, enforcing 'no silent failure' at the construction phase.");

        // ZNT0021-ZNT0023: Validation Doctrine
        public static DiagnosticDescriptor ZNT0021ValidatorMustHaveDefinitionProperty => validatorMustHaveDefinitionProperty;
        private static readonly DiagnosticDescriptor validatorMustHaveDefinitionProperty = new(
            id: "ZNT0021",
            title: "IValidator must have a Definition property",
            messageFormat: "The validator type '{0}' must contain a public or internal get-only 'Definition' property",
            category: ValidationDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces that 'IValidator' implementations explicitly expose their 'IValidationDefinition'.");

        public static DiagnosticDescriptor ZNT0022ValidationErrorMustReferenceValidator => validationErrorMustReferenceValidator;
        private static readonly DiagnosticDescriptor validationErrorMustReferenceValidator = new(
            id: "ZNT0022",
            title: "IValidationError must reference its validator",
            messageFormat: "The validation error type '{0}' must contain a public or internal get-only 'ValidationDefinition' property",
            category: ValidationDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces a crucial self-referential link from a validation error to its originating validator's definition.");

        public static DiagnosticDescriptor ZNT0023ValidationContextIsSuccessfulDerived => validationContextIsSuccessfulDerived;
        private static readonly DiagnosticDescriptor validationContextIsSuccessfulDerived = new(
            id: "ZNT0023",
            title: "ValidationContext's IsSuccessful must be derived",
            messageFormat: "The 'IsSuccessful' property on type '{0}' must be a computed property without a setter",
            category: ValidationDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures the 'IsSuccessful' property is a reflection of the validation state, preventing external manipulation.");

        // ZNT0024-ZNT0025: Metadata Doctrine
        public static DiagnosticDescriptor ZNT0024MetadataIsImmutable => metadataIsImmutable;
        private static readonly DiagnosticDescriptor metadataIsImmutable = new(
            id: "ZNT0024",
            title: "IMetadata must be immutable",
            messageFormat: "Type '{0}' implementing IMetadata must be sealed and have get-only properties",
            category: MetadataDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the core immutability doctrine for metadata contracts, crucial for thread safety and predictable state.");

        public static DiagnosticDescriptor ZNT0025MetadataBuilderMustHaveBuildMethod => metadataBuilderMustHaveBuildMethod;
        private static readonly DiagnosticDescriptor metadataBuilderMustHaveBuildMethod = new(
            id: "ZNT0025",
            title: "Metadata builder must have a public Build() method",
            messageFormat: "The metadata builder type '{0}' must contain a public parameterless 'Build()' method",
            category: MetadataDoctrineCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Reinforces the builder pattern's design by ensuring the 'Build()' method is correctly exposed as the canonical finalization step.");
    }
}

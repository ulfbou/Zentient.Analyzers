// <copyright file="ZNT0001_Analyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;

namespace Zentient.Analyzers.ReferenceImpl.Phase1
{
    /// <summary>
    /// Represents a successful result with optional messages and errors.
    /// </summary>
    public sealed record OkResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkResult"/> record.
        /// </summary>
        /// <param name="Messages">
        /// An optional collection of informational messages associated with the result.
        /// If <c>null</c>, an empty list is used.
        /// </param>
        public OkResult(IEnumerable<string>? Messages = null)
        {
            this.Messages = (Messages ?? Enumerable.Empty<string>()).ToList();
        }

        /// <summary>
        /// Gets a value indicating whether the result is successful.
        /// Returns <c>true</c> if there are no errors; otherwise, <c>false</c>.
        /// </summary>
        public bool IsSuccess => Errors?.Any() != true;

        /// <summary>
        /// Gets the error message associated with the result, if any.
        /// Always returns <c>null</c> for <see cref="OkResult"/>.
        /// </summary>
        public string? ErrorMessage => null;

        /// <summary>
        /// Gets a collection of error information objects.
        /// Always returns an empty collection for <see cref="OkResult"/>.
        /// </summary>
        public IEnumerable<IErrorInfo<IErrorDefinition>> Errors => Enumerable.Empty<IErrorInfo<IErrorDefinition>>();

        /// <summary>
        /// Gets the list of informational messages associated with the result.
        /// </summary>
        public IReadOnlyList<string> Messages { get; }
    }
}

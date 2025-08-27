// <copyright file="src/Zentient.Analyzers/Abstractions/IBuildEngine.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Defines the build engine responsible for generating source units from provided seeds.
    /// </summary>
    public interface IBuildEngine
    {
        /// <summary>
        /// Occurs after a source unit has been emitted.
        /// </summary>
        event Action<ISourceUnit>? AfterEmit;

        /// <summary>
        /// Occurs before a source unit is emitted.
        /// </summary>
        event Action<ISourceUnit>? BeforeEmit;

        /// <summary>
        /// Builds source units from the specified seeds.
        /// </summary>
        /// <param name="instructionKeys">The collection of seed keys to build from.</param>
        /// <returns>A read-only list of generated <see cref="ISourceUnit"/> instances.</returns>
        IReadOnlyList<ISourceUnit> Build(IEnumerable<string> instructionKeys);
    }
}

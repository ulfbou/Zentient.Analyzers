// <copyright file="src/Zentient.Analyzers/Registry/StubInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry
{
    internal class StubContext : IStubContext
    {
        public string Key { get; }
        public string Domain { get; }

        public StubContext(string key, string domain)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(domain));
            }

            Key = key;
            Domain = domain;
        }
    }
}

// <copyright file="ZNT0001_Analyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Zentient.Analyzers.Testing.ZNT0001.ResultCompliance;

namespace Zentient.Analyzers.Tests.Testing.ZNT0001
{
    using VerifyCS = CSharpAnalyzerVerifier<ZNT0001ResultComplianceAnalyzer, DefaultVerifier>;

    public class ZNT0001Tests
    {
        [Fact]
        public async Task ReportsSettableIsSuccess()
        {
            await VerifyCS.VerifyAnalyzerAsync("""
                public class {|ZNT0001:MyResult|} {
                    public bool IsSuccess { get; set; }
                }
            """);
        }
    }
}

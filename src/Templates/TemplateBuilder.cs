// <copyright file="TemplateBuilder{TBuilder}.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zentient.Analyzers.Templates
{
    /// <summary>
    /// Abstract base for fluent builders that assemble compilable source strings
    /// for analyzer testing.
    /// </summary>
    public abstract class TemplateBuilder<TBuilder>
        where TBuilder : TemplateBuilder<TBuilder>
    {
        private string _namespace = "Zentient.TestSubjects";
        private readonly List<Func<string, string>> _mutations = new();
        private readonly StringBuilder _sb = new();

        /// <summary>Header boilerplate (usings, etc.).</summary>
        protected abstract string Usings { get; }

        /// <summary>Return compliant implementation templates for this specialization.</summary>
        protected abstract string GetCompliantImplementations(string ns, string baseName);

        public TBuilder WithNamespace(string ns)
        {
            _namespace = ns ?? _namespace;
            return (TBuilder)this;
        }

        public TBuilder AddCompliant(string baseClassName = "Subject")
        {
            _sb.Append(Usings);
            _sb.Append(GetCompliantImplementations(_namespace, baseClassName));
            return (TBuilder)this;
        }

        public TBuilder WithMutation(Func<string, string> mutation)
        {
            _mutations.Add(mutation);
            return (TBuilder)this;
        }

        public string Build()
        {
            var code = _sb.ToString();
            foreach (var m in _mutations)
                code = m(code);
            return code;
        }
    }
}

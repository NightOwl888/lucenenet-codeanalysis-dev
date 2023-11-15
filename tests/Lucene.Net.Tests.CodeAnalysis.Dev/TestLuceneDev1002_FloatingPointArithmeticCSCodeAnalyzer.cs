﻿using Lucene.Net.CodeAnalysis.Dev;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper;

namespace Lucene.Net.Tests.CodeAnalysis.Dev
{
    public class TestLuceneDev1002_FloatingPointArithmeticCSCodeAnalyzer : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new LuceneDev1002_FloatingPointArithmeticCSCodeAnalyzer();
        }

        //No diagnostics expected to show up
        [Test]
        public void TestEmptyFile()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void TestDiagnostic_Float_ToString()
        {
            var test = @"
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;

        public class MyClass
        {
            private readonly float float1 = 1f;
            private readonly float float2 = 3.14f;

            public void MyMethod()
            {
                long foo = 33;
                var result = ((double)float1 * (double)float2)) / foo;
            }
       }
       ";

            var expected = new DiagnosticResult
            {
                Id = LuceneDev1002_FloatingPointArithmeticCSCodeAnalyzer.DiagnosticId,
                Message = string.Format("'{0}' may fail due to floating point precision issues on .NET Framework and .NET Core prior to version 3.0. Floating point type arithmetic needs to be checked on x86 in .NET Framework and may require extra casting.", "float1.ToString"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                                    new DiagnosticResultLocation("Test0.cs", 15, 33)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}

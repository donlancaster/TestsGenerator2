using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using TestsGeneratorLib;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {

        private List<SyntaxNode> _roots;

        private List<string> _methodList;

        [TestInitialize]
        public void Setup()
        {
            _methodList = new List<string> { "GetTraceResultTest", "StartTraceTest", "StopTraceTest" };

            Generator generator = new Generator();
            List<TestUnit> testFiles = generator.CreateTests(File.ReadAllText("..\\..\\..\\..\\Files\\Tracer.cs"));

            _roots = new List<SyntaxNode>();
            foreach (TestUnit testFile in testFiles)
            {
                _roots.Add(CSharpSyntaxTree.ParseText(testFile.Source).GetRoot());
            }
        }



    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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



        [TestMethod]
        public void FilesAmountTest()
        {
            Assert.AreEqual(1, _roots.Count, "�������� ���������� ��������� ������: "+_roots.Count);
        }

        [TestMethod]
        public void UsingTest()
        {

        }

        [TestMethod]
        public void MethodTest()
        {

        }


        [TestMethod]
        public void ClassTest()
        {
            IEnumerable<ClassDeclarationSyntax> classes =
              _roots[0].DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            Assert.AreEqual(1, classes.Count(), "�������� ���������� �������: "+ classes.Count());

            foreach (ClassDeclarationSyntax syntax in classes)
            {
                Assert.IsTrue(syntax.Modifiers.Any(SyntaxKind.PublicKeyword), "� ������ ��������� ����������� �������");
                Assert.AreEqual(3, syntax.Members.Count, "�������� ���������� ������� � ������");
                Assert.AreEqual("TracerClassTest", syntax.Identifier.ValueText,
                    "�������� ���������� ������ �� ������� � ���������");
                Assert.AreEqual("[TestClass]", syntax.AttributeLists.ToString(),
                    "������� ���������� ������ �� ������ � ���������");
            }

        }
    }
}

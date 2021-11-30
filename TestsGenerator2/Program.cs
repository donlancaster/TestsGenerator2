﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestsGeneratorLib;

namespace TestsGenerator2
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator();
            ExecutionDataflowBlockOptions blockOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 };
            TransformBlock<string, string> readFilesBlock = new TransformBlock<string, string>
            (
                async filePath => await File.ReadAllTextAsync(filePath),
                blockOptions
            );

            TransformManyBlock<string, TestUnit> createTestsBlock = new TransformManyBlock<string, TestUnit>
            (
            async sourceCode => await Task.Run(() => generator.CreateTests(sourceCode).ToArray()),
             blockOptions
            );


            ActionBlock<TestUnit> saveTestsBlock = new ActionBlock<TestUnit>
            (
                async testsFile =>
                    await File.WriteAllTextAsync("..\\..\\..\\..\\GeneratedTests\\" + testsFile.FileName,
                        testsFile.SourceCode),
                blockOptions
            );

        }
    }
}

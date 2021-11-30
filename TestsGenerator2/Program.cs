using System;
using System.IO;
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


        }
    }
}

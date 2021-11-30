using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib
{
   public  class TestUnit
    {
        public string FileName { get; }
        public string Source { get; }
        public TestUnit (string filePath, string source)
        {
            FileName = filePath;
            Source = source;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib
{
   public  class TestUnit
    {
        public string FilePath { get; }
        public string Source { get; }
        public TestUnit (string filePath, string source)
        {
            FilePath = filePath;
            Source = source;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakerLib.Interfaces;
namespace PluginLongGenerator
{
    public class LongGenerator : IGenerator
    {
        public Type GeneratorType => typeof(long);

        public object Create()
        {
            Random random = new Random();
            return unchecked((long)(random.NextDouble() * ulong.MaxValue));
        }
    }
}

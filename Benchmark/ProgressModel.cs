using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class ProgressModel
    {
        public double Speed { get; private set; }

        public ProgressModel(double bytesPerSecond)
        {
            Speed = bytesPerSecond / 1024 / 1024;
        }
    }
}

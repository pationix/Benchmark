using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class RunerModel
    {
        public int BlockSize { get; private set; }
        public System.IO.DriveInfo DriveInfo { get; private set; }

        public RunerModel(int blockSize, System.IO.DriveInfo driveInfo)
        {
            this.BlockSize = blockSize;
            this.DriveInfo = driveInfo;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class SpeedTestResultHeader
    {
        public int SpeedTestResultHeaderId { get; set; }
        public string DeviceName { get; set; }
        public ICollection<SpeedTestResult> Results { get; set; }
        public double AvgSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double MinSpeed { get; set; }
        public string Date { get; set; }
    }

    public class SpeedTestResult
    {
        public int SpeedTestResultId { get; set; }
        public int SpeedTestResultHeaderId { get; set; }
        public SpeedTestResultHeader SpeedTestResultHeader { get; set; }
        public double Value { get; set; }
    }
}

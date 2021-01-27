using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class Device
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public List<LogicalDisk> LogicalDisks { get; set; } = new List<LogicalDisk>();
    }
    public class LogicalDisk
    {
        public string PhysicalName { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string VolumeName { get;  set; }
        public string DiskName { get;  set; }
        public string DiskModel { get;  set; }
        public string DiskInterface { get;  set; }
        public ushort[] Capabilities { get;  set; }
        public bool MediaLoaded { get;  set; }
        public string MediaType { get;  set; }
        public uint MediaSignature { get;  set; }
        public string MediaStatus { get;  set; }
        public string DriveName { get;  set; }
        public string DriveId { get;  set; }
        public bool DriveCompressed { get;  set; }
        public uint DriveType { get;  set; }
        public string FileSystem { get;  set; }
        public ulong FreeSpace { get;  set; }
        public ulong TotalSpace { get;  set; }
        public uint DriveMediaType { get;  set; }
        public string VolumeSerial { get;  set; }
    }
}

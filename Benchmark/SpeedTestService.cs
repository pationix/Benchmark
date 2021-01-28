using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmark
{
    public class SpeedTestService
    {
        private RunerModel runerModel;
        private BackgroundWorker backgroundWorker;


        internal void Run(RunerModel runerModel, BackgroundWorker backgroundWorker)
        {
            this.runerModel = runerModel;
            this.backgroundWorker = backgroundWorker;
            MainLoop();
        }

        private void MainLoop()
        {
            while (!backgroundWorker.CancellationPending)
                ServiceRun();
            Console.WriteLine("SpeedTestService is Cancelled.");
        }

        private void ServiceRun()
        {
            string path = Path.Combine(runerModel.DriveInfo.RootDirectory.FullName, "SpeedTestBenchmark" + DateTime.Now.Ticks + ".dat");
            string s = Path.GetPathRoot(Environment.SystemDirectory);
            if (s == runerModel.DriveInfo.RootDirectory.FullName)
            {
                path = Path.Combine(Path.GetTempPath(), "SpeedTestBenchmark" + DateTime.Now.Ticks + ".dat");
            }
            byte[] data = new byte[1024];
            

            double bytesPerSecond = 0;

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();

                 for (int i = 0; i < 1024 ; i++) 
                    fs.Write(data, 0, data.Length);

                fs.Flush();

                watch.Stop();

                bytesPerSecond = ((data.Length * 1024) / watch.Elapsed.TotalSeconds);
            }
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {

            }
            backgroundWorker.ReportProgress(0, new ProgressModel(bytesPerSecond));
            Thread.Sleep(1000);
        }
    }
}

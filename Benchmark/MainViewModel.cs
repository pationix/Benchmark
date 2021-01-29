using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity;

namespace Benchmark
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private BackgroundWorker backgroundWorker;
        public event PropertyChangedEventHandler PropertyChanged;
        private string min;
        private string max;
        private string avg;
        private string actualSpeed;
        private string selectedDevice;
        private bool isBusy;
        private TimeSpan timeSpan;
        public ObservableCollection<Parameter> parameters;
        #endregion

        #region Properties
        public ICommand StartCommand { get; set; }
        //public ICommand StopCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value; RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(BtnText));
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }
        public bool IsEnabled
        {
            get { return !isBusy; }
        }
        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<double> Results { get; set; } = new ObservableCollection<double>();
        public string Min { get { return min; } set { min = value; RaisePropertyChanged(); } }
        public string Max { get { return max; } set { max = value; RaisePropertyChanged(); } }
        public string Avg { get { return avg; } set { avg = value; RaisePropertyChanged(); } }
        public string ActualSpeed { get { return actualSpeed; } set { actualSpeed = value; RaisePropertyChanged(); } }
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();
        public string SelectedDevice { get { return selectedDevice; } set { selectedDevice = value; RaisePropertyChanged(); ChangeSelectedDevice(); } }

        private void ChangeSelectedDevice()
        {
            try
            {
                Device device = Devices.FirstOrDefault(x => x.DeviceId == SelectedDevice);
                if (device == null)
                {
                    Parameters = new ObservableCollection<Parameter>();
                    return;
                }

                Parameters = new ObservableCollection<Parameter>(device.LogicalDisks.Select(x => new Parameter
                {
                    FileSystem = x.FileSystem,
                    DriveId = x.DriveId,
                    FreeSpace = ((double)x.FreeSpace / 1024 / 1024 / 1024).ToString("n2") + " GB",
                    TotalSpace = ((double)x.TotalSpace / 1024 / 1024 / 1024).ToString("n2") + " GB",
                    BusySpace = ((double)(x.TotalSpace - (double)x.FreeSpace) / 1024 / 1024 / 1024).ToString("n2") + " GB"
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public string BtnText { get { return !IsBusy ? "Test" : "Zatrzymaj"; } }
        public string TimeSpan { get { return timeSpan == null ? "00:00" : timeSpan.Minutes.ToString().PadLeft(2, '0') + ":" + timeSpan.Seconds.ToString().PadLeft(2, '0'); } }

        public ObservableCollection<Parameter> Parameters { get { return parameters; } set { parameters = value; RaisePropertyChanged(); } }
        #endregion

        #region Ctor
        public MainViewModel()
        {
            Init();
        }
        #endregion

        #region Methods 
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Init()
        {
            Commands();
            LoadDiskInfo();
            //serie danych
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Dysk 1",
                    Values = new ChartValues<ObservableValue>()
                }
            };

        }

        private void Commands()
        {
            StartCommand = new RelayCommand(Start, CanStart);
            //StopCommand = new RelayCommand(Stop, CanStop);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
        }
        private bool CanStart(object arg)
        {
            return !string.IsNullOrEmpty(selectedDevice);
        }
        //private bool CanStop(object arg)
        //{
        //    return IsBusy;
        //}

        private bool CanRefresh(object arg)
        {
            return !IsBusy;
        }


        private void Start(object obj)
        {
            try
            {
                if (IsBusy)
                {
                    Stop(obj);
                    return;
                }

                Device device = Devices.FirstOrDefault(x => x.DeviceId == selectedDevice);
                var drives = System.IO.DriveInfo.GetDrives().ToList();
                DriveInfo driveInfo = null;
                foreach (var x in device.LogicalDisks)
                {
                    driveInfo = drives.FirstOrDefault(y => y.Name == x.DriveId + "\\" && y.IsReady && y.AvailableFreeSpace > 500 * 1024 * 1024);
                    if (driveInfo != null)
                        break;
                }

                if (driveInfo == null)
                {
                    MessageBox.Show("Nie można odnaleść dostępnej partycji.");
                    return;
                }



                IsBusy = true;
                Results.Clear();
                SeriesCollection[0].Values.Clear();
                string disk = Devices.FirstOrDefault(x => x.DeviceId == selectedDevice)?.Name ?? "Dysk 1";
                SeriesCollection[0] = 
                new LineSeries
                {
                    Title = disk,
                    Values = new ChartValues<ObservableValue>()
                };
                SetResults();
                backgroundWorker = new BackgroundWorker();
                RunerModel model = new RunerModel(65536, driveInfo);
                backgroundWorker.DoWork += BackgroundWorker_DoWork;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                RunTimer();
                backgroundWorker.RunWorkerAsync(model);
            }
            catch (Exception e)
            {

            }
        }
        private DateTime start = DateTime.Now;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void RunTimer()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            start = DateTime.Now;
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            timeSpan = DateTime.Now - start;
            RaisePropertyChanged(nameof(timeSpan));
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            dispatcherTimer.Stop();
        }

        private void Stop(object obj)
        {
            try
            {
                if (backgroundWorker.IsBusy)
                    backgroundWorker.CancelAsync();
                SetResults();


                Device device = Devices.FirstOrDefault(x => x.DeviceId == selectedDevice);
                if (device == null)
                    return;
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    context.SpeedTestResultHeaders.Add(new SpeedTestResultHeader { DeviceName = device.Name, Date = DateTime.Now, DateString = DateTime.Now.ToString("yyyyMMdd"), MinSpeed = Results.Min(), MaxSpeed = Results.Max(), AvgSpeed = Results.Average(), Results = Results.Select(x => new SpeedTestResult { Value = x }).ToList() });
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {

            }
        }


        private void Refresh(object obj)
        {
            LoadDiskInfo();
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            new SpeedTestService().Run(e.Argument as RunerModel, sender as BackgroundWorker);
        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ProgressModel model = e.UserState as ProgressModel;
                Results.Add(model.Speed);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SeriesCollection[0].Values.Add(new ObservableValue(model.Speed));
                    if (SeriesCollection[0].Values.Count > 30)
                        SeriesCollection[0].Values.RemoveAt(0);
                });
                ActualSpeed = model.Speed.ToString("n2") + "MB/s";
                SetResults();
            }
            catch (Exception ex)
            {

            }
        }
        private void SetResults()
        {
            if (Results.Count == 0)
            {
                Min = "";
                Max = "";
                Avg = "";
                ActualSpeed = "";
            }
            else
            {
                Min = Results.Min().ToString("n2") + "MB/s";
                Max = Results.Max().ToString("n2") + "MB/s";
                Avg = Results.Average().ToString("n2") + "MB/s";
            }
        }

        private void LoadDiskInfo()
        {
            try
            {
                List<Device> devices = new List<Device>();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                var driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                foreach (ManagementObject d in driveQuery.Get())
                {
                    Device device = new Device();
                    devices.Add(device);
                    device.DeviceId = d.Properties["DeviceId"].Value.ToString();

                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        var logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                        var logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
                        foreach (ManagementObject ld in logicalDriveQuery.Get())
                        {
                            LogicalDisk logicalDisk = new LogicalDisk();
                            device.LogicalDisks.Add(logicalDisk);

                            logicalDisk.PhysicalName = Convert.ToString(d.Properties["Name"].Value); // \\.\PHYSICALDRIVE2
                            logicalDisk.DiskName = Convert.ToString(d.Properties["Caption"].Value); // WDC WD5001AALS-xxxxxx
                            logicalDisk.DiskModel = Convert.ToString(d.Properties["Model"].Value); // WDC WD5001AALS-xxxxxx
                            logicalDisk.DiskInterface = Convert.ToString(d.Properties["InterfaceType"].Value); // IDE
                            logicalDisk.Capabilities = (UInt16[])d.Properties["Capabilities"].Value; // 3,4 - random access, supports writing
                            logicalDisk.MediaLoaded = Convert.ToBoolean(d.Properties["MediaLoaded"].Value); // bool
                            logicalDisk.MediaType = Convert.ToString(d.Properties["MediaType"].Value); // Fixed hard disk media
                            logicalDisk.MediaSignature = Convert.ToUInt32(d.Properties["Signature"].Value); // int32
                            logicalDisk.MediaStatus = Convert.ToString(d.Properties["Status"].Value); // OK

                            logicalDisk.DriveName = Convert.ToString(ld.Properties["Name"].Value); // C:
                            logicalDisk.DriveId = Convert.ToString(ld.Properties["DeviceId"].Value); // C:
                            logicalDisk.DriveCompressed = Convert.ToBoolean(ld.Properties["Compressed"].Value);
                            logicalDisk.DriveType = Convert.ToUInt32(ld.Properties["DriveType"].Value); // C: - 3
                            logicalDisk.FileSystem = Convert.ToString(ld.Properties["FileSystem"].Value); // NTFS
                            logicalDisk.FreeSpace = Convert.ToUInt64(ld.Properties["FreeSpace"].Value); // in bytes
                            logicalDisk.TotalSpace = Convert.ToUInt64(ld.Properties["Size"].Value); // in bytes
                            logicalDisk.DriveMediaType = Convert.ToUInt32(ld.Properties["MediaType"].Value); // c: 12
                            logicalDisk.VolumeName = Convert.ToString(ld.Properties["VolumeName"].Value); // System
                            logicalDisk.VolumeSerial = Convert.ToString(ld.Properties["VolumeSerialNumber"].Value); // 12345678
                        }
                    }
                    device.Name = device.LogicalDisks.FirstOrDefault()?.DiskName.Trim().Replace("ATA Device", "").Replace("USB Device", "");
                    device.Name += $" ( {string.Join("  ", device.LogicalDisks.Select(x => x.DriveName + "\\"))} )";
                }
                string selectedDevice = SelectedDevice;
                Devices.Clear();
                foreach (var item in devices)
                {
                    Devices.Add(item);
                }
                if (string.IsNullOrEmpty(selectedDevice) || Devices.Any(x => x.DeviceId == selectedDevice) == false)
                    SelectedDevice = Devices.FirstOrDefault()?.DeviceId;
                else
                    SelectedDevice = Devices.FirstOrDefault(x => x.DeviceId == selectedDevice)?.DeviceId;
                //DriveInfo[] driveInfos = DriveInfo.GetDrives();
            }
            catch (Exception e)
            {

            }
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
        }
        #endregion
        public class Parameter
        {
            public string DriveId { get; set; }
            public string FileSystem { get; set; }
            public string FreeSpace { get; set; }
            public string TotalSpace { get; set; }
            public string BusySpace { get; set; }
        }
    }
}


using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Data.Entity;
using System.Windows;

namespace Benchmark
{
    public class ArchiveViewModel : INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private string selectedDevice;
        private DateTime date = DateTime.Today;
        private SeriesCollection seriesCollection;
        private List<string> labels = new List<string>();
        #endregion

        #region Properties
        public SeriesCollection SeriesCollection { get { return seriesCollection; } set { seriesCollection = value; RaisePropertyChanged(); } }
        public List<string> Labels { get { return labels; } set { labels = value; RaisePropertyChanged(); } }
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();
        public string SelectedDevice { get { return selectedDevice; } set { selectedDevice = value; RaisePropertyChanged(); Reload(); } }
        public DateTime Date { get { return date; } set { date = value; RaisePropertyChanged(); Reload(); } }

        private void Reload()
        {
            try
            {
                SeriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<ObservableValue>()
                }
            };
                Labels.Clear();


                if (!string.IsNullOrEmpty(SelectedDevice))
                {

                    List<SpeedTestResultHeader> data = null;
                    string searchDate = Date.ToString("yyyyMMdd");
                    using (ApplicationDbContext dbContext = new ApplicationDbContext())
                    {
                        data = dbContext.SpeedTestResultHeaders.Where(x => x.DateString == searchDate && x.DeviceName == selectedDevice).ToList();
                    }
                    if (data == null || data.Count == 0)
                    {
                        MessageBox.Show("Brak zapisanych pomiarów.");
                    }
                    else
                    {
                        SeriesCollection[0] = new ColumnSeries { Title = selectedDevice, Values = new ChartValues<double>(data.OrderBy(x => x.Date).Select(x => x.AvgSpeed)) };
                        Labels = data.OrderBy(x=>x.Date).Select(x => x.Date.ToString("hh:mm")).ToList();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        public Func<double, string> Formatter { get; set; } = value => value.ToString("N");
        #region Ctor
        public ArchiveViewModel()
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
            //serie danych
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                var data = dbContext.SpeedTestResultHeaders.Select(x => x.DeviceName).Distinct().ToList();
                foreach (var item in data)
                {
                    Devices.Add(new Device { Name = item });
                }
            }

            SeriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<ObservableValue>()
                }
            };
        }

        #endregion
    }
}

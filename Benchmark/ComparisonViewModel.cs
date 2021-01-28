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
    public class ComparisonViewModel : INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private string selectedDevice;
        private DateTime date = DateTime.Today;
        private SeriesCollection seriesCollection;
        private List<string> labels;
        #endregion

        #region Properties
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public SeriesCollection SeriesCollection { get { return seriesCollection; } set { seriesCollection = value; RaisePropertyChanged(); } }
        public List<string> Labels { get { return labels; } set { labels = value; RaisePropertyChanged(); } }
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();
        public string SelectedDevice { get { return selectedDevice; } set { selectedDevice = value; RaisePropertyChanged(); } }
        public DateTime Date { get { return date; } set { date = value; RaisePropertyChanged(); Reset(); } }

        private void Reset()
        {
            SeriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "Porównanie",
                    Values = new ChartValues<ObservableValue>()
                }
            };
            Labels.Clear();
            SelectedDevices.Clear();
        }

        Dictionary<string, double> SelectedDevices = new Dictionary<string, double>();
        #endregion

        public Func<double, string> Formatter { get; set; } = value => value.ToString("N");
        #region Ctor
        public ComparisonViewModel()
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
                    Title = "Porównanie",
                    Values = new ChartValues<ObservableValue>()
                }
            };

        }

        private void Commands()
        {
            AddCommand = new RelayCommand(Add, CanAdd);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
        }
        private bool CanAdd(object arg)
        {
            return !string.IsNullOrEmpty(selectedDevice) && !SelectedDevices.ContainsKey(selectedDevice);
        }

        private bool CanDelete(object arg)
        {
            return !string.IsNullOrEmpty(selectedDevice) && SelectedDevices.ContainsKey(selectedDevice);
        }

        private void Add(object obj)
        {
            try
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
                    SelectedDevices.Add(SelectedDevice, data.Average(x => x.AvgSpeed));
                    SeriesCollection[0] = new ColumnSeries { Title = "Porównanie", Values = new ChartValues<double>(SelectedDevices.Select(x => x.Value)) };
                    Labels = SelectedDevices.Select(x => x.Key).ToList();
                }
            }
            catch (Exception e)
            {

            }
        }



        private void Delete(object obj)
        {
            try
            {
                SelectedDevices.Remove(SelectedDevice);
                SeriesCollection[0] = new ColumnSeries { Title = "Porównanie", Values = new ChartValues<double>(SelectedDevices.Select(x => x.Value)) };
                Labels = SelectedDevices.Select(x => x.Key).ToList();
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}

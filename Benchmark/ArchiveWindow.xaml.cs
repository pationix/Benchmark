using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Benchmark
{
    /// <summary>
    /// Logika interakcji dla klasy ArchiveWindow.xaml
    /// </summary>
    public partial class ArchiveWindow : Window
    {
        private ZoomingOptions _zoomingMode;
        public ArchiveWindow()
        {


            InitializeComponent();

            ZoomingMode = ZoomingOptions.X;
            XFormatter = val => new DateTime((long)val).ToString("hh:mm:ss");
            YFormatter = val => val.ToString("C");

            DataContext = this;
        }
            public ZoomingOptions ZoomingMode
            {
                get { return _zoomingMode; }
                set
                {
                    _zoomingMode = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}


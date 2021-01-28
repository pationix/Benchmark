using System;
using System.Collections.Generic;
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
using LiveCharts.Wpf;

namespace Benchmark
{
    /// <summary>
    /// Logika interakcji dla klasy ComparisonWindow.xaml
    /// </summary>
    public partial class ComparisonWindow : Window
    {
        public ComparisonWindow()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "28.01.2021",
                    Values = new ChartValues<double> { 39, 50,  }
                }
            };

            Labels = new[] { "H", "C" };
            Formatter = value => value.ToString("N");

            DataContext = this;
        }
      
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private SeriesCollection SeriesCollection { get; set; }
    }
}

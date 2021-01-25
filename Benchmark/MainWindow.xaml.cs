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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Charts;
using LiveCharts.Definitions.Series;

namespace Benchmark
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var doubleValues = new ChartValues<double> { 1 };
            var intValues = new ChartValues<int> { 1};

            //the observable value class is really helpful, it notifies the chart to update
            //every time the ObservableValue.Value property changes
            var observableValues = new ChartValues<LiveCharts.Defaults.ObservableValue>
            {
                new LiveCharts.Defaults.ObservableValue(1), //initializes Value property as 1

            };
            var myValues = new LiveCharts.ChartValues<double>
            {
              10, //index 0
              6,  //index 1
              9,  //index 2
              2,  //index 3
              7   //index 4
            };
            SeriesCollection = new SeriesCollection
            {
                new LiveCharts.Wpf.LineSeries
                {
                    Values = new ChartValues<double> { 3, 5, 7, 4, 5 }
                }
            };
        }

        public SeriesCollection SeriesCollection { get; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
    

}

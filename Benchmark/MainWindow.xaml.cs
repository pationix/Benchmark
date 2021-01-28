using System.Windows;

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
        }

        private void OpenArchive(object sender, RoutedEventArgs e)
        {
            ArchiveWindow archiveWindow = new ArchiveWindow { Owner = this };
            archiveWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComparisonWindow comparisonWindow = new ComparisonWindow { Owner = this };
            comparisonWindow.ShowDialog();
        }
    }
}

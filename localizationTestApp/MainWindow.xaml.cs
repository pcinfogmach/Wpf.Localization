using Localization;
using System.Windows;

namespace localizationTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LocalizationExtension.Locale = LocalizationExtension.Locale == "en" ? "he" : "en";
        }
    }
}

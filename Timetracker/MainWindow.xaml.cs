using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timetracker.ViewModels;
using Timetracker.Views;
using static Timetracker.App;

namespace Timetracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            MainViewControl.DataContext = viewModel;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            Width = screenWidth * 0.8;
            Height = screenHeight * 0.8;

            ThemeManager.SetTheme("LightTheme"); // oder "LightTheme"

        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDarkTheme)
            {
                ThemeManager.SetTheme("LightTheme");
                ThemeToggleButton.Content = "🌙 Dark Mode";
            }
            else
            {
                ThemeManager.SetTheme("DarkTheme");
                ThemeToggleButton.Content = "☀️ Light Mode";
            }

            isDarkTheme = !isDarkTheme;
        }


    }
}
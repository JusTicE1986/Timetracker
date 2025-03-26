using System.Configuration;
using System.Data;
using System.Windows;

namespace Timetracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static class ThemeManager
        {
            public static void SetTheme(string themeName)
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative)
                };

                var existing = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true);
                if (existing != null)
                    Application.Current.Resources.MergedDictionaries.Remove(existing);

                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
    }

}

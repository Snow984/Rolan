using Rolan.Helpers;
using Rolan.Models;
using System.Windows;

namespace Rolan
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _settings;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            DataContext = _settings;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AutoStartCheck_Checked(object sender, RoutedEventArgs e)
        {
            LauncherHelper.ToggleAutoStart(true);
        }

        private void AutoStartCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            LauncherHelper.ToggleAutoStart(false);
        }
    }
}

using NMTimeTracker.Model;
using NMTimeTracker.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel settingsViewModel)
            {
                settingsViewModel.Settings.Save();
            }
            Close();
        }

        private void CopySettingsPath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm && !string.IsNullOrEmpty(vm.SettingsFilePath))
                System.Windows.Clipboard.SetText(vm.SettingsFilePath);
        }

        private void ShowSettingsPath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm && !string.IsNullOrEmpty(vm.SettingsFilePath))
                Process.Start("explorer.exe", $"/select,\"{vm.SettingsFilePath}\"");
        }

        private void CopyDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm && !string.IsNullOrEmpty(vm.DatabaseFilePath))
                System.Windows.Clipboard.SetText(vm.DatabaseFilePath);
        }

        private void ShowDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm && !string.IsNullOrEmpty(vm.DatabaseFilePath))
                Process.Start("explorer.exe", $"/select,\"{vm.DatabaseFilePath}\"");
        }

        private void GenerateHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenerateHistoryWindow();
            window.ShowDialog();
        }
    }
}

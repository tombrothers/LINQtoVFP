/*
 * LINQ to VFP 
 * http://linqtovfp.codeplex.com/
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System;
using System.Windows;
using LINQPad.Extensibility.DataContext;
using VfpClient;
using System.Xml.Linq;

namespace LinqToVfpLinqPadDriver {
    public partial class OptionsDialog : Window {
        private IConnectionInfo _connectionInfo;

        public OptionsDialog(IConnectionInfo connectionInfo) {
            _connectionInfo = connectionInfo;
            DataContext = connectionInfo;
            InitializeComponent();
            Pluralization.IsChecked = !connectionInfo.DynamicSchemaOptions.NoPluralization;
            Capitalize.IsChecked = !connectionInfo.DynamicSchemaOptions.NoCapitalization;
            var singularizeElement = _connectionInfo.DriverData.Element("Singularize");

            if (singularizeElement == null) {
                Singularize.IsChecked = true;
            }
            else {
                Singularize.IsChecked = Convert.ToBoolean(singularizeElement.Value);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e) {
            var currentDirectory = Environment.CurrentDirectory;
            var dialog = new Microsoft.Win32.OpenFileDialog() {
                Title = "Locate Database Container",
                DefaultExt = ".dbc",
                Filter = "VPF Database Container|*.dbc"
            };

            if (dialog.ShowDialog() == true) {
                if (!System.IO.Path.GetExtension(dialog.FileName).Equals(".dbc", StringComparison.InvariantCultureIgnoreCase)) {
                    MessageBox.Show("Invalid file.", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else {
                    txtDataPath.Text = dialog.FileName;
                }
            }

            Environment.CurrentDirectory = currentDirectory;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            try {
                using (var connection = new VfpConnection(txtDataPath.Text)) {
                    connection.Open();
                    connection.Close();
                }
            }
            catch (Exception) {
                MessageBox.Show("Invalid connection string.", Title, MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
            }

            if (e.Handled) {
                return;
            }

            _connectionInfo.DynamicSchemaOptions.NoPluralization = !Pluralization.IsChecked.Value;
            _connectionInfo.DynamicSchemaOptions.NoCapitalization = !Capitalize.IsChecked.Value;

            var singularizeElement = _connectionInfo.DriverData.Element("Singularize");

            if (singularizeElement == null) {
                _connectionInfo.DriverData.Add(new XElement("Singularize", Singularize.IsChecked.Value));
            }
            else {
                singularizeElement.Value = Singularize.IsChecked.Value.ToString();
            }

            DialogResult = true;
        }
    }
}
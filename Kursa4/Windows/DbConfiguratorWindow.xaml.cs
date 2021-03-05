using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kursa4.Windows
{
    /// <summary>
    /// Логика взаимодействия для DbConfiguratorWindow.xaml
    /// </summary>
    public partial class DbConfiguratorWindow : Window
    {
        
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public string ConnectionString;

        public DbConfiguratorWindow(string? connectionString)
        {
            InitializeComponent();

            if (connectionString != null)
            {
                ConnectionString = connectionString;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            if (ConnectionString != null)
            {
                SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder(ConnectionString);

                //ServerField.Text = stringBuilder.DataSource.Split(@"\".ToCharArray())[0];
                if (stringBuilder.DataSource.Split(@"\".ToCharArray()).Length == 2)
                {
                    ServerField.Text = stringBuilder.DataSource.Split(@"\".ToCharArray())[0];
                    InstanceField.Text = stringBuilder.DataSource.Split(@"\".ToCharArray())[1];
                }
                else
                {
                    ServerField.Text = stringBuilder.DataSource;
                }
                if (stringBuilder.DataSource.Split(@",".ToCharArray()).Length == 2)
                {

                    TCPPortField.Text = stringBuilder.DataSource.Split(@",".ToCharArray())[1];
                }

                DatabaseField.Text = stringBuilder.InitialCatalog;
                ApplicationField.Text = stringBuilder.ApplicationName;
                ConnectionTimeoutFieldName.Text = stringBuilder.ConnectTimeout.ToString();
                SSLField.IsChecked = stringBuilder.Encrypt;

                PoolField.IsChecked = stringBuilder.Pooling;
                PoolField_Click(PoolField, null);

                MinPoolSizeField.Text = stringBuilder.MaxPoolSize.ToString();
                MaxPoolSize.Text = stringBuilder.MaxPoolSize.ToString();
                ConnectionLifetimeField.Text = stringBuilder.LoadBalanceTimeout.ToString();

                WindowsAuthentificationTypeField.IsChecked = stringBuilder.IntegratedSecurity;
                SQLServerAuthentificationTypeField.IsChecked = !stringBuilder.IntegratedSecurity;

                WindowsAuthentificationTypeField_Click(null, null);

                if (stringBuilder.IntegratedSecurity)
                {
                    UserNameField.Text = stringBuilder.UserID;
                    PasswordField.Text = stringBuilder.Password;
                }


            }
        }

        private void PoolField_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            PoolSettingsContainer.IsEnabled = checkBox.IsChecked == true;
        }

        private void WindowsAuthentificationTypeField_Click(object sender, RoutedEventArgs e)
        {
            AuthentificationSettingsContainer.IsEnabled = WindowsAuthentificationTypeField.IsChecked == true;
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.DataSource = string.IsNullOrWhiteSpace(InstanceField.Text) ? ServerField.Text : ServerField.Text + @"\" + InstanceField.Text;
            int tcpPort = 0;
            if (int.TryParse(TCPPortField.Text, out tcpPort))
            {
                stringBuilder.DataSource += "," + tcpPort.ToString();
            }
            stringBuilder.InitialCatalog = DatabaseField.Text;
            stringBuilder.ApplicationName = ApplicationField.Text;
            int timeout = 0;
            if (int.TryParse(ConnectionTimeoutFieldName.Text, out timeout))
            {
                stringBuilder.ConnectTimeout = timeout;
            }
            stringBuilder.Encrypt = SSLField.IsChecked == true;

            stringBuilder.Pooling = PoolField.IsChecked == true;
            int minpoolsize = 0;
            int maxpoolsize = 0;
            int connectionlifetime = 0;
            stringBuilder.MinPoolSize = int.TryParse(MinPoolSizeField.Text, out minpoolsize) ? minpoolsize : 0;
            stringBuilder.MaxPoolSize = int.TryParse(MaxPoolSize.Text, out maxpoolsize) ? maxpoolsize : 0;
            stringBuilder.LoadBalanceTimeout = int.TryParse(ConnectionLifetimeField.Text, out connectionlifetime) ? connectionlifetime : 0;

            if (WindowsAuthentificationTypeField.IsChecked == true)
            {
                stringBuilder.IntegratedSecurity = true;
            }
            else
            {
                stringBuilder.IntegratedSecurity = false;
                stringBuilder.UserID = UserNameField.Text;
                stringBuilder.Password = PasswordField.Text;
            }
            stringBuilder.MultipleActiveResultSets = true;
            
            ConnectionString = stringBuilder.ToString();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти без сохранения?",
                "Подтверждение",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }
        }
    }
}

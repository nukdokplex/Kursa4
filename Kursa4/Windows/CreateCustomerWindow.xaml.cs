using Kursa4.Entitities;
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
    /// Логика взаимодействия для CreateCustomerWindow.xaml
    /// </summary>
    public partial class CreateCustomerWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public CreateCustomerWindow()
        {
            InitializeComponent();
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RealNameField.Text))
            {
                MessageBox.Show(
                    "Введите корректное реальное имя пользователя!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(NickNameField.Text))
            {
                MessageBox.Show(
                    "Введите корректный никнейм пользователя!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var query = (from user in App.DB.Users
                         where user.NickName == NickNameField.Text
                         select user);

            if (query.Count() > 0)
            {
                MessageBox.Show(
                    "К сожадению, данный никнейм занят, попробуйте другой!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            User customer = new User();
            customer.RealName = RealNameField.Text;
            customer.NickName = NickNameField.Text;
            customer.Password = string.IsNullOrWhiteSpace(PasswordField.Text) ? null : PasswordField.Text;
            customer.EMail = string.IsNullOrWhiteSpace(EMailField.Text) ? null : EMailField.Text;
            customer.Phone = string.IsNullOrWhiteSpace(PhoneField.Text) ? null : PhoneField.Text;
            customer.Type = 4;

            App.DB.Users.Add(customer);
            App.DB.SaveChanges();

            MessageBox.Show(
                "Ппокупатель успешно создан",
                "Успех!",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}

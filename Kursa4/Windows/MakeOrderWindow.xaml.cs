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
    /// Логика взаимодействия для MakeOrderWindow.xaml
    /// </summary>
    public partial class MakeOrderWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public MakeOrderWindow()
        {
            InitializeComponent();
            ReloadCustomers();
            ReloadAvailableProducts();
        }

        private void ReloadCustomers()
        {
            var query = (from user in App.DB.Users
                         select user);
            CustomersDataGrid.ItemsSource = query.ToList();
            
        }


        private void ReloadAvailableProducts()
        {
            var query = (from product in App.DB.Products
                         select product);
            AvailableProductsDataGrid.ItemsSource = query.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void SelectProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeselectProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeadlineAtField_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker.SelectedDate.HasValue && datePicker.SelectedDate.Value > DateTime.Now)
            {
                MessageBox.Show(
                    "Срок выполнения заказа должен быть хотя бы на день позже текущей даты!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                datePicker.SelectedDate = null;
            }
        }
    }
}

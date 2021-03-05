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

namespace Kursa4
{
    /// <summary>
    /// Логика взаимодействия для ChangeOrderStatusWindow.xaml
    /// </summary>
    public partial class ChangeOrderStatusWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public long OrderID;
        public ChangeOrderStatusWindow(long orderID)
        {
            InitializeComponent();
            this.OrderID = orderID;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            (from order in App.DB.Orders
             where order.ID == OrderID
             select order).Single().Status = (OrderStatusComboBox.SelectedItem as OrderStatu).ID;
            App.DB.SaveChanges();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            var query = (from status in App.DB.OrderStatus
                         select status);

            OrderStatusComboBox.ItemsSource = query.ToList();
            OrderStatusComboBox.SelectedItem = (from order in App.DB.Orders
                                                where order.ID == OrderID
                                                select order).Single().OrderStatu;
            
        }
    }
}

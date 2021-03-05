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
    /// Логика взаимодействия для CreateProductWindow.xaml
    /// </summary>
    public partial class CreateProductWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public CreateProductWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            string productName = ProductNameField.Text;
            string productSKU = ProductSKUField.Text;
            string productPrice = ProductPriceField.Text;
            string productCount = ProductCountField.Text;

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show(
                    "Проеверьте поле Наименование!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(productSKU))
            {
                MessageBox.Show(
                    "Проеверьте поле Наименование!",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            decimal productPriceD;
            if (string.IsNullOrWhiteSpace(productPrice) || !decimal.TryParse(productPrice, out productPriceD))
            {
                MessageBox.Show(
                    "Проеверьте поле Цена! Оно должно быть числовым с плавающей запятой (точность 2 знака).",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            int productCountI;
            if (string.IsNullOrWhiteSpace(productCount) || !int.TryParse(productCount, out productCountI))
            {
                MessageBox.Show(
                    "Проеверьте поле Количество! Оно должно быть целочисленным.",
                    "Внимание!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                ProductCountField.Text = "0";
                return;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show(
                "Вы действительно хотите добавить введенные данные в базу данных?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                
                Product product = new Product();
                product.Author = App.CurrentUser.ID;
                product.CreatedAt = DateTime.Now;
                product.Name = productName;
                product.SKU = productSKU;
                product.Count = productCountI;
                product.Price = productPriceD;
                App.DB.Products.Add(product);
                App.DB.SaveChanges();
                
                this.Close();
            }
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

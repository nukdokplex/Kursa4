using Kursa4.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Kursa4.Windows
{
    public partial class MakeOrderWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public List<SelectedProduct> SelectedProducts { get; set; }
        public MakeOrderWindow()
        {
            InitializeComponent();
            ReloadCustomers();
            ReloadAvailableProducts();
            SelectedProducts = new List<SelectedProduct>();
            SelectedProductsDataGrid.ItemsSource = SelectedProducts;
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

        private void ReloadSelectedProducts()
        {
            SelectedProductsDataGrid.ItemsSource = SelectedProducts;
            SelectedProductsDataGrid.Items.Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void SelectProduct_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableProductsDataGrid.SelectedItem != null)
            {
                Product currentProduct = AvailableProductsDataGrid.SelectedItem as Product;

                currentProduct = (from product in App.DB.Products
                                  where product.ID == currentProduct.ID
                                  select product).Single();
                if (!SelectProductCount.Value.HasValue)
                {
                    MessageBox.Show(
                        "Невалдиное значение количества выбираемого товара",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                if (currentProduct.Count < SelectProductCount.Value.Value)
                {
                    MessageBox.Show(
                        "Значение количества выбранного товара превышает запасы на складе.",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                for (int i = 0; i <= SelectedProducts.Count -1; i++)
                {
                    if (currentProduct.ID == SelectedProducts[i].ID)
                    {
                        SelectedProducts[i].Count += SelectProductCount.Value.Value;
                        currentProduct.Count -= SelectProductCount.Value.Value;
                        App.DB.SaveChanges();
                        ReloadSelectedProducts();
                        ReloadAvailableProducts();
                        return;
                    }
                    continue;
                }
                
                
                SelectedProducts.Add(new SelectedProduct(
                    currentProduct.ID, 
                    currentProduct.Name, 
                    currentProduct.Price, 
                    SelectProductCount.Value.Value
                    )
                );
                currentProduct.Count -= SelectProductCount.Value.Value;
                App.DB.SaveChanges();
                ReloadSelectedProducts();
                ReloadAvailableProducts();
                
            }
        }

        private void DeselectProduct_Click(object sender, RoutedEventArgs e)
        {
            //TODO Implement product deselection
            /*if (SelectedProductsDataGrid.SelectedItem != null)
            {
                Product currentProduct = SelectedProductsDataGrid.SelectedItem as Product;
                int index = -1;

                for (int i = 0; i <= SelectedProducts.Count - 1; i++)
                {
                    if (SelectedProducts[i].ID == currentProduct.ID)
                    {
                        index = i;
                        currentProduct = SelectedProducts[i];
                        break;
                    }
                }

                if (index == -1)
                {
                    return;
                }

                SelectedProducts.RemoveAt(index);
                
                for (int i = 0; i <= AvailableProductsDataGrid.Items.Count -1; i++)
                {
                    if (currentProduct.ID == (AvailableProductsDataGrid.Items[i] as Product).ID)
                    {
                        (from product in App.DB.Products
                         where product.ID == currentProduct.ID
                         select product).Single().Count += currentProduct.Count;
                        break;
                    }
                }
                App.DB.SaveChanges();
                ReloadSelectedProducts();
                ReloadAvailableProducts();
            }*/
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO Implement order save
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO Implement agreement MessageBox
            this.Close();
        }

        private void DeadlineAtField_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODELETE This code isn't working at all
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

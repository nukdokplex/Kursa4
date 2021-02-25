using Kursa4.Entitities;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kursa4.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : ConcretePage
    {
        public ProductsPage()
        {
            InitializeComponent();
            
            
        }

        public void ReloadProducts()
        {
            
            if (!string.IsNullOrWhiteSpace(ProductSearchNameField.Text))
            {
                var query = (from product in App.DB.Products where product.Name.ToLower().Contains(ProductSearchNameField.Text) select product);
                IEnumerable<Product> products = query.ToList<Product>();

                ProductsDataGrid.ItemsSource = products;
            }
            else
            {
                var query = (from product in App.DB.Products select product);
                IEnumerable<Product> products = query.ToList<Product>();

                ProductsDataGrid.ItemsSource = products;
            }
        }

        private void ConcretePage_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadProducts();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DB.SaveChanges();
                MessageBox.Show(
                    "Данные успешно сохранены!",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception exception)
            {
                
                MessageBox.Show(
                    $"Произошла ошибка сохранения: {exception.Message}",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                
                
            }
            ReloadProducts();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            App.DB = new dbConnection();
        }

        private void MakeProductButton_Click(object sender, RoutedEventArgs e)
        {
            CreateProductWindow createProductWindow = new CreateProductWindow();
            createProductWindow.ShowDialog();
            ReloadProducts();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Product productRow = ProductsDataGrid.SelectedItem as Product;
            Product product = (from p in App.DB.Products
                               where p.ID == productRow.ID
                               select p).Single();
            

            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите удалить этот товар?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                App.DB.Products.Remove(product);
                App.DB.SaveChanges();
                ReloadProducts();
            }

            
        }

        private void ProductSearchNameField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReloadProducts();
            }
        }

       

    }
}

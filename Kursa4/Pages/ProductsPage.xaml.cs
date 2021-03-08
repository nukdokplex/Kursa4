using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Kursa4.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

                ProductsDataGrid.ItemsSource = query.ToList();
            }
            else
            {
                var query = (from product in App.DB.Products select product);

                ProductsDataGrid.ItemsSource = query.ToList();
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "PDF-Документ|*.pdf";
            sfd.FilterIndex = 0;
            sfd.Title = "Экспорт списка товаров в PDF";
            sfd.RestoreDirectory = true;

            System.Windows.Forms.DialogResult result = sfd.ShowDialog();
            if (!result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                return;
            }
            PdfWriter writer = new PdfWriter(sfd.FileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);


            document.SetFont(App.DefaultPdfFont);

            document.Add(new Paragraph("Список товаров")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20)
            );

            document.Add(new Paragraph($"на {DateTime.Now.ToString()}")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(15)
            ); 


            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

           

            Table products = new Table(3, true);

            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Наименование"))
            );
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Количество"))
            );
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Цена"))
            );
           

            List<Product> productList= (from product in App.DB.Products
                                                select product).ToList();
            decimal total = 0;

            foreach (Product product in productList)
            {
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(product.Name))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(product.Count.ToString()))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(product.Price.ToString() + " руб."))
                );
                

            }
            document.Add(products);
           
            document.Close();
        }
    }
}

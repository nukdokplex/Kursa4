using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Kursa4.Entitities;
using System;
using System.Collections.Generic;
using System.IO;
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

            if (File.Exists(sfd.FileName))
            {
                File.Delete(sfd.FileName);
            }

            PdfDocument pdfDoc;
            try
            {
                pdfDoc = new PdfDocument(new PdfWriter(sfd.FileName));
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"При попытке закрытия документа произошла ошибка: \"{exception.Message}\". Невозможно установить причину возникновения этой ошибки.",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }



            Document document = new Document(pdfDoc);

            document.SetFont(App.GetDefaultPdfFont());

            document.Add(new Paragraph("Список товаров")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20)
            );

            document.Add(new Paragraph($"на {DateTime.Now.ToString()}")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(15)
            );

            document.Add(new Paragraph());


            Table products = new Table(4, true);
            document.Add(products);
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("ID"))
            );
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Наименование"))
            );
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Цена"))
            );
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Количество"))
            );
            products.Flush();
            List<Product> productsList = (from product in App.DB.Products
                                          select product).ToList();

            foreach (Product product in productsList)
            {
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph($"#{product.ID.ToString()}"))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(product.Name))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph($"{product.Price} руб."))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph($"{product.Count} шт."))
                );
                products.Flush();
            }

            products.Complete();

            try
            {
                document.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"При попытке закрытия документа произошла ошибка: \"{exception.Message}\". Невозможно установить причину возникновения этой ошибки.",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
        }
    }
}

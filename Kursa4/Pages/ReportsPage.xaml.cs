using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Kursa4.Entitities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kursa4.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : ConcretePage
    {
        private IQueryable<Product> ProductsQuery;
        private IQueryable<Order> OrdersQuery;

        public ReportsPage()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
            var query = (from p in App.DB.Products
                         select p);
        }

        private void ReloadOrders()
        {
            OrdersQuery = (from order in App.DB.Orders
                           orderby order.CreatedAt ascending
                           select order);

            if (OrderStartDatePicker.SelectedDate.HasValue)
            {
                OrdersQuery = OrdersQuery.Where(order => order.CreatedAt >= OrderStartDatePicker.SelectedDate);
            }

            if (OrderEndDatePicker.SelectedDate.HasValue)
            {
                OrdersQuery = OrdersQuery.Where(order => order.CreatedAt <= OrderEndDatePicker.SelectedDate);
            }

            OrdersDataGrid.ItemsSource = OrdersQuery.ToList<Order>();

            OrdersDataGrid.Items.Refresh();
        }

        private void ReloadProducts()
        {
            ProductsQuery = (from product in App.DB.Products
                             select product);

            if (!string.IsNullOrWhiteSpace(ProductSearchNameField.Text))
            {
                ProductsQuery = ProductsQuery.Where(product => product.Name.Contains(ProductSearchNameField.Text));
            }

            ProductsDataGrid.ItemsSource = ProductsQuery.ToList<Product>();

            ProductsDataGrid.Items.Refresh();
        }

        private void ExportPDFOrders_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "PDF-Документ|*.pdf";
            sfd.FilterIndex = 0;
            sfd.Title = "Экспорт списка заказов в PDF";
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

            document.Add(new Paragraph("Список заказов")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20)
            );

            StringBuilder dateBuilder = new StringBuilder();

            if (OrderStartDatePicker.SelectedDate.HasValue)
            {
                dateBuilder.Append("c ")
                    .Append(OrderStartDatePicker.SelectedDate.ToString());
            }

            if (OrderEndDatePicker.SelectedDate.HasValue)
            {
                dateBuilder.Append(" по ")
                    .Append(OrderEndDatePicker.SelectedDate.ToString());
            }

            string dates = dateBuilder.ToString();

            if (string.IsNullOrEmpty(dates))
            {
                dates = "за все время";
            }

            document.Add(new Paragraph(dates)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(15)
            );

            document.Add(new Paragraph());

            Table orders = new Table(6, true);
            document.Add(orders);
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("ID"))
            );
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Дата создания"))
            );
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Срок исполнения"))
            );
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Покупатель"))
            );
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Продавец"))
            );
            orders.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Сумма заказа"))
            );
            orders.Flush();
            List<Order> ordersList = OrdersQuery.ToList<Order>();

            decimal ordersTotal = 0;

            foreach (Order order in ordersList)
            {
                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph($"#{order.ID.ToString()}"))
                );

                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(order.CreatedAt.ToString()))
                );

                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(order.DeadlineAt.HasValue ? order.DeadlineAt.ToString() : "---"))
                );

                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(order.User.RealName))
                );

                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(order.User1.RealName))
                );

                decimal total = 0;

                List<OrderProduct> orderProducts = (from op in App.DB.OrderProducts
                                                    where op.Order == order.ID
                                                    select op).ToList<OrderProduct>();

                foreach (OrderProduct op in orderProducts)
                {
                    total += op.Product1.Price * op.Count;
                }

                orders.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph($"{total} руб."))
                );

                ordersTotal += total;

                /*products.AddCell(new Cell(1, 1)
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
                products.Flush();*/
            }

            orders.Complete();

            document.Add(new Paragraph("Итого: " + ordersTotal.ToString() + " руб.")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                .SetFontSize(15)
                .SetBold()
            );

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

        private void ExportPDFProducts_Click(object sender, RoutedEventArgs e)
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
            List<Product> productsList = ProductsQuery.ToList();

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

        private void OrderStartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadOrders();
        }

        private void OrderEndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadOrders();
        }

        private void ConcretePage_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadOrders();
            ReloadProducts();
        }

        private void ProductSearchNameField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                ReloadProducts();
            }
        }
    }
}
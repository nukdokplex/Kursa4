using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Kursa4.Entitities;
using Kursa4.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Kursa4.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public User CurrentCustomer { get; set; }
        public User CurrentMerchant { get; set; }
        public Order CurrentOrder { get; set; }
        public List<SelectedProduct> SelectedProducts { get; set; }
        public OrderDetailsWindow(long orderID)
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentOrder = (from order in App.DB.Orders
                            where order.ID == orderID
                            select order).Single();
            CurrentCustomer = (from user in App.DB.Users
                               where user.ID == CurrentOrder.Customer
                               select user).Single();
            CurrentMerchant = (from user in App.DB.Users
                               where user.ID == CurrentOrder.Merchant
                               select user).Single();
            List<OrderProduct> orderProducts = (from op in App.DB.OrderProducts
                                                where op.Order == CurrentOrder.ID
                                                select op).ToList();

            SelectedProducts = new List<SelectedProduct>();
            foreach (OrderProduct op in orderProducts)
            {
                SelectedProducts.Add(new SelectedProduct(op.Product, op.Product1.Name, op.Product1.Price, op.Count));
            }

            ReloadAvailableProducts();
            ReloadSelectedProducts();
            ReloadTotal();

            DeadlineAtField.SelectedDate = CurrentOrder.DeadlineAt;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);


        }

        private void ReloadSelectedProducts()
        {
            SelectedProductsDataGrid.ItemsSource = SelectedProducts;
            SelectedProductsDataGrid.Items.Refresh();
        }

        private void ReloadAvailableProducts()
        {
            var query = (from product in App.DB.Products
                         select product);

            AvailableProductsDataGrid.ItemsSource = query.ToList();
            AvailableProductsDataGrid.Items.Refresh();
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

                for (int i = 0; i <= SelectedProducts.Count - 1; i++)
                {
                    if (currentProduct.ID == SelectedProducts[i].ID)
                    {
                        SelectedProducts[i].Count += SelectProductCount.Value.Value;
                        currentProduct.Count -= SelectProductCount.Value.Value;
                        App.DB.SaveChanges();
                        ReloadSelectedProducts();
                        ReloadAvailableProducts();
                        ReloadTotal();
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
                ReloadTotal();

            }
        }

        public void ReloadTotal()
        {
            decimal total = 0;

            for (int i = 0; i <= SelectedProductsDataGrid.Items.Count - 1; i++)
            {
                SelectedProduct product = SelectedProductsDataGrid.Items[i] as SelectedProduct;
                total += product.Count * product.Price;
            }

            TotalPriceField.Text = total.ToString();
        }

        private void DeselectProduct_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProductsDataGrid.SelectedItem != null)
            {
                SelectedProduct currentProduct = SelectedProductsDataGrid.SelectedItem as SelectedProduct;
                for (int i = 0; i <= SelectedProducts.Count - 1; i++)
                {
                    if (SelectedProducts[i].ID == currentProduct.ID)
                    {
                        currentProduct = SelectedProducts[i];
                        SelectedProducts.RemoveAt(i);
                        break;
                    }
                }

                (from product in App.DB.Products
                 where product.ID == currentProduct.ID
                 select product).Single().Count += currentProduct.Count;

                App.DB.SaveChanges();
                ReloadSelectedProducts();
                ReloadAvailableProducts();
                ReloadTotal();
            }
        }

        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentOrder.DeadlineAt = DeadlineAtField.SelectedDate;

            List<OrderProduct> orderProductsToRemove = (from op in App.DB.OrderProducts
                                                        where op.Order == CurrentOrder.ID
                                                        select op).ToList();
            App.DB.OrderProducts.RemoveRange(orderProductsToRemove);
            App.DB.SaveChanges();

            foreach (object product in SelectedProductsDataGrid.Items)
            {
                SelectedProduct selectedProduct = product as SelectedProduct;
                OrderProduct orderProduct = new OrderProduct();
                orderProduct.Order = CurrentOrder.ID;
                orderProduct.Product = selectedProduct.ID;
                orderProduct.Count = selectedProduct.Count;
                App.DB.OrderProducts.Add(orderProduct);
            }

            App.DB.SaveChanges();

            MessageBox.Show(
                "Изменение заказа прошло успешно!",
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "PDF-Документ|*.pdf";
            sfd.FilterIndex = 0;
            sfd.Title = "Экспорт отчета в PDF";
            sfd.RestoreDirectory = true;

            System.Windows.Forms.DialogResult result = sfd.ShowDialog();
            if (!result.Equals(System.Windows.Forms.DialogResult.OK)){
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

            document.SetFont(App.DefaultPdfFont);

            document.Add(new Paragraph("ЗАКАЗ")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20)
            );

            document.Add(new Paragraph("#"+CurrentOrder.ID)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(15)
            );

            document.Add(new Paragraph("Дата создания: " + CurrentOrder.CreatedAt.Value.ToString())
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)             
            );
            if (CurrentOrder.DeadlineAt.HasValue)
            {
                document.Add(new Paragraph("Срок выполнения: " + CurrentOrder.CreatedAt.Value.ToString())
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                );
            }

            

            Table users = new Table(2, true);
            document.Add(users);
            users.AddHeaderCell(new Cell().Add(new Paragraph("Покупатель").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
            users.AddHeaderCell(new Cell().Add(new Paragraph("Продавец").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
            users.Flush();
            users.AddCell(new Cell().Add(new Paragraph(CurrentCustomer.RealName)));
            users.AddCell(new Cell().Add(new Paragraph(CurrentMerchant.RealName)));
            //users.Flush();
            users.Complete();



            document.Add(new Paragraph());


            Table products = new Table(4, true);
            document.Add(products);
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
            products.AddHeaderCell(new Cell(1, 1)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new Paragraph("Сумма"))
            );
            products.Flush();
            List<OrderProduct> orderProducts = (from op in App.DB.OrderProducts
                                                where op.Order == CurrentOrder.ID
                                                select op).ToList();
            decimal total = 0;

            foreach(OrderProduct op in orderProducts)
            {
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(op.Product1.Name))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(op.Count.ToString()))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph(op.Product1.Price.ToString() + " руб."))
                );
                products.AddCell(new Cell(1, 1)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .Add(new Paragraph((op.Product1.Price*op.Count).ToString()))
                );
                products.Flush();
                total += op.Product1.Price * op.Count;
            }

            
            //products.Flush();
            products.Complete();
            
            document.Add(new Paragraph("Итого: " + total.ToString()+ " руб.")
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
    }
}

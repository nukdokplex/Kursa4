using Kursa4.Entitities;
using Kursa4.Windows;
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
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : ConcretePage
    {
        protected bool RemoveLastBackEntry;

        public ProductsPage? productsPage = null;
        public OrdersPage? ordersPage = null;
        public ReportsPage? reportsPage = null;
        
        public User CurrentUser {
            get;
            set;
        }

        public StartPage(bool removeLastBackEntry)
        {
            InitializeComponent();

            RemoveLastBackEntry = removeLastBackEntry;
            CurrentUser = App.CurrentUser;
            this.DataContext = this;
            
        }

        private void ConcretePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (RemoveLastBackEntry)
            {
                NavigationService.RemoveBackEntry();
                RemoveLastBackEntry = false;
            }
            //MessageBox.Show(CurrentUser.RealName);

            var elements = MenuContainer.Children;

            foreach (UIElement element in elements)
            {
                Button button = element as Button;
                List<long> allowedUserTypes = new List<long>();
                string[] tags = (button.Tag as string).Split(new[] { ',' });
                foreach (string tag in tags)
                {
                    if (string.IsNullOrEmpty(tag))
                        continue;
                    allowedUserTypes.Add(long.Parse(tag));
                }

                if (!allowedUserTypes.Contains(CurrentUser.Type))
                {
                    button.IsEnabled = false;
                    //button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void GoToOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            if (ordersPage == null)
            {
                ordersPage = new OrdersPage();
            }
            NavigationService.Navigate(ordersPage);
        }

        private void GoToProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsPage == null)
            {
                productsPage = new ProductsPage();
            }
            NavigationService.Navigate(productsPage);
        }

        private void GoToReportsButton_Click(object sender, RoutedEventArgs e)
        {
            if (reportsPage == null)
            {
                reportsPage = new ReportsPage();
            }

            this.NavigationService.Navigate(reportsPage);
        }

        private void GoToCreateCustomerWindow_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomerWindow createCustomerWindow = new CreateCustomerWindow();
            createCustomerWindow.ShowDialog();
        }
    }
}

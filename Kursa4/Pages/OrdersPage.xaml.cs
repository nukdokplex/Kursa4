﻿using Kursa4.Entitities;
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
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : ConcretePage
    {
        public OrdersPage()
        {
            InitializeComponent();
            ReloadOrders();
        }

        private void ReloadOrders()
        {
            var query = (from order in App.DB.Orders
                         select order);
            if (StartDatePicker.SelectedDate.HasValue)
            {
                query = (from order in App.DB.Orders
                         where order.CreatedAt >= StartDatePicker.SelectedDate.Value
                         select order);
            }
            if (EndDatePicker.SelectedDate.HasValue)
            {
                query = (from order in App.DB.Orders
                         where order.CreatedAt <= StartDatePicker.SelectedDate.Value
                         select order);
            }
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                query = (from order in App.DB.Orders
                         where order.CreatedAt >= StartDatePicker.SelectedDate.Value &&
                         order.CreatedAt <= EndDatePicker.SelectedDate.Value
                         select order);
            }
            OrdersDataGrid.ItemsSource = query.ToList<Order>();

        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            var changeOrderStatusWindow = new ChangeOrderStatusWindow();
            changeOrderStatusWindow.ShowDialog();
            
        }

        private void OrderInfoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var makeOrderButton = new MakeOrderWindow();
            makeOrderButton.ShowDialog();
        }
    }
}
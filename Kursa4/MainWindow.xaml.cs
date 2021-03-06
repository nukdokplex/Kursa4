﻿using Kursa4.Pages;
using Kursa4.Utils;
using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Kursa4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HeaderPage Header;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            App.DB.Database.Connection.ConnectionString = Properties.Settings.Default.ConnectionString;

            //Header
            Header = new HeaderPage(ContentFrame);
            HeaderFrame.Navigate(Header);

            //Content
            ContentFrame.Navigate(new AuthPage());

            
        }

    }
}
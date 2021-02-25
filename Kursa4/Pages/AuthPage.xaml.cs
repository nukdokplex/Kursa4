﻿using Kursa4.Entitities;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : ConcretePage
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(LoginField.Text))
            {
                MessageBox
                    .Show(
                        "Поле логина пользователя не должно быть пустым или состоять из последовательности пробелов!",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                return;
            }

            if (String.IsNullOrWhiteSpace(GetPassword()))
            {
                MessageBox
                    .Show(
                        "Поле пароля пользователя не должно быть пустым или состоять из последовательности пробелов!",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                return;
            }

            App.CurrentUser = LoginUser(LoginField.Text, GetPassword());

            if (App.CurrentUser == null)
            {
                MessageBox
                    .Show(
                        "Пользователь с таким логином и (или) паролем не найден. Проверьте правильность введенных данных!",
                        "Ошибка!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                return;
            }

            /*MessageBox
                .Show(
                    "Успешный вход!",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );*/

            NavigationService.Navigate(new StartPage(true));
            NavigationService.RemoveBackEntry();
        }

        

        private User? LoginUser(string login, string password)
        {
            try
            {
                User currentUser = (from User in App.DB.Users
                                    where User.NickName == login &&
                                    User.Password == password
                                    select User).Single<User>();
                return currentUser;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
            {
                PasswordField.Visibility = Visibility.Collapsed;
                PasswordFieldUnmasked.Visibility = Visibility.Visible;
                PasswordFieldUnmasked.Text = PasswordField.Password;
            }
            else
            {
                PasswordField.Visibility = Visibility.Visible;
                PasswordFieldUnmasked.Visibility = Visibility.Collapsed;
                PasswordField.Password = PasswordFieldUnmasked.Text;
            }
        }

        private string GetPassword()
        {
            if (ShowPassword.IsChecked.Value)
            {
                return PasswordFieldUnmasked.Text;
            }
            else
            {
                return PasswordField.Password;
            }
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace coiled_tubing_app
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Validasi sederhana
            if (string.IsNullOrEmpty(UsernameTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowLoginError("Please enter both username and password.");
                return;
            }

            // Contoh validasi login (ganti dengan logika autentikasi yang sesuai)
            if (ValidateLogin(UsernameTextBox.Text, PasswordBox.Password))
            {
                ShowLoginSuccess("Login successful!");
            }
            else
            {
                ShowLoginError("Invalid username or password.");
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            // Contoh validasi sederhana - ganti dengan logika autentikasi yang sebenarnya
            // Misalnya: validasi dengan database, API, atau Active Directory
            return username.Equals("admin", StringComparison.CurrentCultureIgnoreCase) && password == "password123";
        }

        private async void ShowLoginError(string message)
        {
            ContentDialog errorDialog = new()
            {
                Title = "Login Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        private async void ShowLoginSuccess(string message)
        {
            ContentDialog successDialog = new ContentDialog()
            {
                Title = "Login Successful",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        // Event handler untuk Enter key
        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginButton_Click(sender, null);
            }
        }

        private void UsernameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                PasswordBox.Focus(FocusState.Programmatic);
            }
        }

    }
}
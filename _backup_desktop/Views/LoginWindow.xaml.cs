using ERPCore.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Input;

namespace ERPCore.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = App.ServiceProvider.GetService<IApiService>();

            Loaded += LoginWindow_Loaded;
            MouseDown += LoginWindow_MouseDown;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void LoginWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformLoginAsync();
        }

        private async Task PerformLoginAsync()
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Por favor, preencha todos os campos.");
                return;
            }

            LoginButton.IsEnabled = false;
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            try
            {
                var success = await _apiService.LoginAsync(username, password);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError("Usuário ou senha inválidos.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erro de conexão: {ex.Message}");
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLoginAsync();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            base.OnKeyDown(e);
        }

        public string Username { get; set; }
    }
}
using System.Windows;
using ERPCore.Desktop.Services;

namespace ERPCore.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Por favor, preencha todos os campos.");
                return;
            }

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

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
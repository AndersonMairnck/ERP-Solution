using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ERPCore.Desktop.Services;
using ERPCore.Desktop.Views;

namespace ERPCore.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurar Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Mostrar tela de login primeiro
            var loginWindow = ServiceProvider.GetService<LoginWindow>();
            if (loginWindow.ShowDialog() == true)
            {
                // Se login bem-sucedido, mostrar main window
                var mainWindow = ServiceProvider.GetService<MainWindow>();
                mainWindow.Show();
            }
            else
            {
                // Se login cancelado, fechar aplicação
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApiService, ApiService>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<PDVWindow>();
            services.AddTransient<ProductsWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Startup já configurado acima
        }
    }
}
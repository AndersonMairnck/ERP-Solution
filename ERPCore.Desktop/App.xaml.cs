using System.Windows;
using ERPCore.Desktop.Views;

namespace ERPCore.Desktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var loginWindow = new LoginWindow();
            //if (loginWindow.ShowDialog() == true)
            //{
            //    var mainWindow = new MainWindow();
            //    mainWindow.Show();
            //}
            //else
            //{
            //    Shutdown();
            //}


            var loginWindow = new MainWindow();
            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }


        }
    }
}